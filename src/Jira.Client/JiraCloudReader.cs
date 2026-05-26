using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using EnterpriseAgentAccelerator.Jira.Client.Models;
using Microsoft.Extensions.Options;

namespace EnterpriseAgentAccelerator.Jira.Client;

public sealed class JiraCloudReader : IJiraReader
{
    private readonly HttpClient _httpClient;
    private readonly JiraOptions _options;

    public JiraCloudReader(HttpClient httpClient, IOptions<JiraOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;

        if (!string.IsNullOrWhiteSpace(_options.BaseUrl))
            _httpClient.BaseAddress = new Uri(_options.BaseUrl.TrimEnd('/') + "/");

        if (!string.IsNullOrWhiteSpace(_options.Email) && !string.IsNullOrWhiteSpace(_options.ApiToken))
        {
            var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_options.Email}:{_options.ApiToken}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);
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
        var risks = new List<string>();
        if (blocked > 0) risks.Add($"{blocked} issue(s) bloqueadas.");
        if (high > 0) risks.Add($"{high} issue(s) de prioridad alta/crítica.");
        return new JiraSprintSummary(projectKey, sprintName, issues.Count, issues.Count(i => i.Status == "Done"), issues.Count(i => i.Status == "In Progress"), blocked, high, risks.ToArray());
    }

    private async Task<IReadOnlyList<JiraIssue>> SearchAsync(string jql, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(new { jql, maxResults = 50, fields = new[] { "summary", "status", "priority", "assignee", "parent", "labels", "updated" } });
        using var content = new StringContent(payload, Encoding.UTF8, "application/json");
        using var response = await _httpClient.PostAsync("rest/api/3/search", content, cancellationToken);
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

        var issues = new List<JiraIssue>();
        foreach (var issue in doc.RootElement.GetProperty("issues").EnumerateArray())
        {
            var fields = issue.GetProperty("fields");
            var key = issue.GetProperty("key").GetString() ?? "UNKNOWN";
            var labels = fields.TryGetProperty("labels", out var labelsElement)
                ? labelsElement.EnumerateArray().Select(x => x.GetString() ?? string.Empty).Where(x => x.Length > 0).ToArray()
                : [];
            issues.Add(new JiraIssue(
                key,
                key.Split('-')[0],
                fields.GetProperty("summary").GetString() ?? string.Empty,
                fields.GetProperty("status").GetProperty("name").GetString() ?? string.Empty,
                fields.TryGetProperty("priority", out var priority) && priority.ValueKind != JsonValueKind.Null ? priority.GetProperty("name").GetString() ?? "Medium" : "Medium",
                fields.TryGetProperty("assignee", out var assignee) && assignee.ValueKind != JsonValueKind.Null ? assignee.GetProperty("displayName").GetString() ?? "Unassigned" : "Unassigned",
                fields.TryGetProperty("parent", out var parent) && parent.ValueKind != JsonValueKind.Null ? parent.GetProperty("key").GetString() ?? string.Empty : string.Empty,
                fields.TryGetProperty("parent", out parent) && parent.ValueKind != JsonValueKind.Null ? parent.GetProperty("fields").GetProperty("summary").GetString() ?? string.Empty : string.Empty,
                "openSprints()",
                labels.Contains("blocked", StringComparer.OrdinalIgnoreCase),
                0,
                fields.TryGetProperty("updated", out var updated) && updated.ValueKind == JsonValueKind.String ? DateTimeOffset.Parse(updated.GetString()!) : DateTimeOffset.UtcNow,
                labels));
        }

        return issues;
    }

    private static string Escape(string value) => value.Replace("\"", "", StringComparison.OrdinalIgnoreCase);
}
