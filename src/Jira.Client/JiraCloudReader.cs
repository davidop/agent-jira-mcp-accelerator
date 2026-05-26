using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using EnterpriseAgentAccelerator.Jira.Client.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EnterpriseAgentAccelerator.Jira.Client;

public sealed class JiraCloudReader : IJiraReader
{
    private readonly HttpClient _httpClient;
    private readonly JiraOptions _options;
    private readonly ILogger<JiraCloudReader> _logger;

    private static readonly string[] SearchFields =
    [
        "summary",
        "status",
        "statuscategorychangedate",
        "priority",
        "assignee",
        "parent",
        "labels",
        "updated",
        "created",
        "customfield_10020"
    ];

    public JiraCloudReader(HttpClient httpClient, IOptions<JiraOptions> options, ILogger<JiraCloudReader> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;

        if (!string.IsNullOrWhiteSpace(_options.BaseUrl))
            _httpClient.BaseAddress = new Uri(_options.BaseUrl.TrimEnd('/') + "/");

        if (!string.IsNullOrWhiteSpace(_options.Email) && !string.IsNullOrWhiteSpace(_options.ApiToken))
        {
            var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_options.Email}:{_options.ApiToken}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);
        }

        if (!_httpClient.DefaultRequestHeaders.Accept.Any(h => h.MediaType == "application/json"))
        {
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }

    public Task<IReadOnlyList<JiraIssue>> GetProjectIssuesAsync(string projectKey, CancellationToken cancellationToken = default) =>
        SearchAsync($"project = {Escape(projectKey)} ORDER BY updated DESC", cancellationToken);

    public Task<IReadOnlyList<JiraIssue>> GetBlockedIssuesAsync(string projectKey, CancellationToken cancellationToken = default) =>
        SearchAsync($"project = {Escape(projectKey)} AND (status = Blocked OR labels in (blocked)) ORDER BY priority DESC", cancellationToken);

    public Task<IReadOnlyList<JiraIssue>> GetIssuesByAssigneeAsync(string assignee, CancellationToken cancellationToken = default) =>
        SearchAsync($"assignee ~ \"{assignee}\" ORDER BY updated DESC", cancellationToken);

    public Task<IReadOnlyList<JiraIssue>> GetIssuesByEpicAsync(string epicKey, CancellationToken cancellationToken = default) =>
        SearchAsync($"parent = {Escape(epicKey)} OR \"Epic Link\" = {Escape(epicKey)} ORDER BY updated DESC", cancellationToken);

    public async Task<JiraSprintSummary> GetSprintSummaryAsync(string projectKey, string? sprint = null, CancellationToken cancellationToken = default)
    {
        var jql = sprint is null
            ? $"project = {Escape(projectKey)} AND sprint in openSprints() ORDER BY priority DESC"
            : $"project = {Escape(projectKey)} AND sprint = \"{sprint}\" ORDER BY priority DESC";
        var issues = await SearchAsync(jql, cancellationToken);
        var sprintName = sprint ?? "openSprints()";
        var blocked = issues.Count(i => i.Blocked || i.Status.Equals("Blocked", StringComparison.OrdinalIgnoreCase));
        var high = issues.Count(i => i.Priority is "High" or "Critical");
        var done = issues.Count(i => i.Status.Equals("Done", StringComparison.OrdinalIgnoreCase));
        var inProgress = issues.Count(i => i.Status.Equals("In Progress", StringComparison.OrdinalIgnoreCase));
        var risks = new List<string>();
        if (blocked > 0) risks.Add($"{blocked} issue(s) bloqueadas.");
        if (high > 0) risks.Add($"{high} issue(s) de prioridad alta/crítica.");
        return new JiraSprintSummary(projectKey, sprintName, issues.Count, done, inProgress, blocked, high, risks.ToArray());
    }

    private async Task<IReadOnlyList<JiraIssue>> SearchAsync(string jql, CancellationToken cancellationToken)
    {
        var maxResults = Math.Clamp(_options.MaxResultsPerPage, 1, 100);
        var maxPages = Math.Max(_options.MaxPages, 1);
        var startAt = 0;
        var page = 0;
        var total = int.MaxValue;
        var issues = new List<JiraIssue>();

        while (startAt < total && page < maxPages)
        {
            page++;
            var payload = JsonSerializer.Serialize(new { jql, startAt, maxResults, fields = SearchFields });
            using var doc = await PostSearchWithRetryAsync(payload, page, cancellationToken);

            var root = doc.RootElement;
            if (root.TryGetProperty("total", out var totalElement) && totalElement.ValueKind == JsonValueKind.Number)
            {
                total = totalElement.GetInt32();
            }

            if (!root.TryGetProperty("issues", out var issuesElement) || issuesElement.ValueKind != JsonValueKind.Array)
            {
                break;
            }

            var received = 0;
            foreach (var issueElement in issuesElement.EnumerateArray())
            {
                issues.Add(ParseIssue(issueElement));
                received++;
            }

            _logger.LogInformation("Jira search page {Page} fetched {Received} issues (startAt {StartAt}, total {Total}).", page, received, startAt, total);

            if (received == 0)
            {
                break;
            }

            startAt += received;
        }

        if (startAt < total)
        {
            _logger.LogWarning("Jira search results truncated at {Returned}/{Total}. Increase Jira:MaxPages or Jira:MaxResultsPerPage if needed.", startAt, total);
        }

        return issues;
    }

    private async Task<JsonDocument> PostSearchWithRetryAsync(string payload, int page, CancellationToken cancellationToken)
    {
        var retries = Math.Max(0, _options.RetryCount);
        for (var attempt = 0; attempt <= retries; attempt++)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, "rest/api/3/search")
            {
                Content = new StringContent(payload, Encoding.UTF8, "application/json")
            };

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                return await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
            }

            if (!ShouldRetry(response.StatusCode) || attempt == retries)
            {
                var body = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Jira search failed for page {Page} with status {StatusCode}. Response: {Body}", page, (int)response.StatusCode, body);
                response.EnsureSuccessStatusCode();
            }

            var delay = GetRetryDelay(attempt, response);
            _logger.LogWarning("Jira search retry for page {Page}. Attempt {Attempt}/{MaxAttempts}. Status {StatusCode}. Waiting {DelayMs} ms.",
                page,
                attempt + 1,
                retries + 1,
                (int)response.StatusCode,
                delay.TotalMilliseconds);
            await Task.Delay(delay, cancellationToken);
        }

        throw new InvalidOperationException("Jira search retry loop finished without a response.");
    }

    private JiraIssue ParseIssue(JsonElement issue)
    {
        var key = issue.TryGetProperty("key", out var keyElement) && keyElement.ValueKind == JsonValueKind.String
            ? keyElement.GetString() ?? "UNKNOWN"
            : "UNKNOWN";

        if (!issue.TryGetProperty("fields", out var fields) || fields.ValueKind != JsonValueKind.Object)
        {
            return new JiraIssue(key, key.Split('-')[0], string.Empty, "Unknown", "Medium", "Unassigned", string.Empty, string.Empty, "openSprints()", false, 0, DateTimeOffset.UtcNow, []);
        }

        var labels = fields.TryGetProperty("labels", out var labelsElement) && labelsElement.ValueKind == JsonValueKind.Array
            ? labelsElement.EnumerateArray().Select(x => x.GetString() ?? string.Empty).Where(x => x.Length > 0).ToArray()
            : [];

        var rawStatus = TryGetNestedString(fields, "status", "name");
        var status = NormalizeStatus(rawStatus);
        var priority = NormalizePriority(TryGetNestedString(fields, "priority", "name"));
        var assignee = TryGetNestedString(fields, "assignee", "displayName") ?? "Unassigned";
        var updatedAt = ParseDate(fields, "updated") ?? DateTimeOffset.UtcNow;
        var createdAt = ParseDate(fields, "created");
        var ageDays = createdAt.HasValue ? Math.Max(0, (int)(DateTimeOffset.UtcNow - createdAt.Value).TotalDays) : 0;

        var epicKey = string.Empty;
        var epicName = string.Empty;
        if (fields.TryGetProperty("parent", out var parent) && parent.ValueKind == JsonValueKind.Object)
        {
            epicKey = parent.TryGetProperty("key", out var parentKey) && parentKey.ValueKind == JsonValueKind.String
                ? parentKey.GetString() ?? string.Empty
                : string.Empty;
            epicName = TryGetNestedString(parent, "fields", "summary") ?? string.Empty;
        }

        var sprint = TryGetSprintName(fields) ?? "openSprints()";
        var blocked = status.Equals("Blocked", StringComparison.OrdinalIgnoreCase) || labels.Contains("blocked", StringComparer.OrdinalIgnoreCase);

        return new JiraIssue(
            key,
            key.Split('-')[0],
            TryGetString(fields, "summary") ?? string.Empty,
            status,
            priority,
            assignee,
            epicKey,
            epicName,
            sprint,
            blocked,
            ageDays,
            updatedAt,
            labels);
    }

    private static string? TryGetString(JsonElement root, string propertyName)
    {
        if (!root.TryGetProperty(propertyName, out var property) || property.ValueKind != JsonValueKind.String)
        {
            return null;
        }

        return property.GetString();
    }

    private static string? TryGetNestedString(JsonElement root, string propertyName, string nestedPropertyName)
    {
        if (!root.TryGetProperty(propertyName, out var property) || property.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        return TryGetString(property, nestedPropertyName);
    }

    private static DateTimeOffset? ParseDate(JsonElement root, string propertyName)
    {
        var raw = TryGetString(root, propertyName);
        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        return DateTimeOffset.TryParse(raw, out var value) ? value : null;
    }

    private static string NormalizeStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            return "Unknown";
        }

        var raw = status.Trim();
        var normalized = raw.ToLowerInvariant();
        if (normalized is "done" or "closed" or "resolved") return "Done";
        if (normalized.Contains("progress", StringComparison.Ordinal) || normalized == "in development") return "In Progress";
        if (normalized.Contains("blocked", StringComparison.Ordinal) || normalized.Contains("imped", StringComparison.Ordinal)) return "Blocked";
        if (normalized is "to do" or "todo" or "open" or "backlog" || normalized.Contains("selected for", StringComparison.Ordinal)) return "To Do";
        return raw;
    }

    private static string NormalizePriority(string? priority)
    {
        if (string.IsNullOrWhiteSpace(priority))
        {
            return "Medium";
        }

        var raw = priority.Trim();
        var normalized = raw.ToLowerInvariant();
        if (normalized is "highest" or "critical") return "Critical";
        if (normalized == "high") return "High";
        if (normalized is "lowest" or "low") return "Low";
        return raw;
    }

    private static string? TryGetSprintName(JsonElement fields)
    {
        if (!fields.TryGetProperty("customfield_10020", out var sprintField) || sprintField.ValueKind != JsonValueKind.Array)
        {
            return null;
        }

        foreach (var sprint in sprintField.EnumerateArray())
        {
            if (sprint.ValueKind == JsonValueKind.Object)
            {
                var sprintName = TryGetString(sprint, "name");
                if (!string.IsNullOrWhiteSpace(sprintName))
                {
                    return sprintName;
                }
            }
        }

        return null;
    }

    private static bool ShouldRetry(System.Net.HttpStatusCode statusCode) =>
        statusCode == System.Net.HttpStatusCode.TooManyRequests || (int)statusCode >= 500;

    private TimeSpan GetRetryDelay(int attempt, HttpResponseMessage response)
    {
        if (response.Headers.TryGetValues("Retry-After", out var values))
        {
            var raw = values.FirstOrDefault();
            if (int.TryParse(raw, out var seconds) && seconds > 0)
            {
                return TimeSpan.FromSeconds(seconds);
            }
        }

        var baseDelayMs = Math.Max(_options.InitialRetryDelayMs, 100);
        var factor = (int)Math.Pow(2, attempt);
        return TimeSpan.FromMilliseconds(baseDelayMs * factor);
    }

    private static string Escape(string value) => value.Replace("\"", "", StringComparison.OrdinalIgnoreCase);
}
