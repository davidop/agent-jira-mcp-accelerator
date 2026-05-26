using System.Text.Json;
using EnterpriseAgentAccelerator.Jira.Client.Models;
using Microsoft.Extensions.Options;

namespace EnterpriseAgentAccelerator.Jira.Client;

public sealed class MockJiraReader : IJiraReader
{
    private readonly JiraOptions _options;
    private IReadOnlyList<JiraIssue>? _cache;

    public MockJiraReader(IOptions<JiraOptions> options) => _options = options.Value;

    public async Task<IReadOnlyList<JiraIssue>> GetProjectIssuesAsync(string projectKey, CancellationToken cancellationToken = default)
    {
        var issues = await LoadAsync(cancellationToken);
        return issues.Where(i => string.Equals(i.ProjectKey, projectKey, StringComparison.OrdinalIgnoreCase)).ToArray();
    }

    public async Task<IReadOnlyList<JiraIssue>> GetBlockedIssuesAsync(string projectKey, CancellationToken cancellationToken = default)
    {
        var issues = await GetProjectIssuesAsync(projectKey, cancellationToken);
        return issues.Where(i => i.Blocked || i.Status.Equals("Blocked", StringComparison.OrdinalIgnoreCase)).ToArray();
    }

    public async Task<IReadOnlyList<JiraIssue>> GetIssuesByAssigneeAsync(string assignee, CancellationToken cancellationToken = default)
    {
        var issues = await LoadAsync(cancellationToken);
        return issues.Where(i => i.Assignee.Contains(assignee, StringComparison.OrdinalIgnoreCase)).ToArray();
    }

    public async Task<IReadOnlyList<JiraIssue>> GetIssuesByEpicAsync(string epicKey, CancellationToken cancellationToken = default)
    {
        var issues = await LoadAsync(cancellationToken);
        return issues.Where(i => i.EpicKey.Equals(epicKey, StringComparison.OrdinalIgnoreCase)).ToArray();
    }

    public async Task<JiraSprintSummary> GetSprintSummaryAsync(string projectKey, string? sprint = null, CancellationToken cancellationToken = default)
    {
        var issues = await GetProjectIssuesAsync(projectKey, cancellationToken);
        var currentSprint = sprint ?? issues.GroupBy(i => i.Sprint).OrderByDescending(g => g.Count()).FirstOrDefault()?.Key ?? "Current Sprint";
        var sprintIssues = issues.Where(i => i.Sprint.Equals(currentSprint, StringComparison.OrdinalIgnoreCase)).ToArray();

        var risks = new List<string>();
        var blocked = sprintIssues.Count(i => i.Blocked || i.Status.Equals("Blocked", StringComparison.OrdinalIgnoreCase));
        var high = sprintIssues.Count(i => i.Priority.Equals("High", StringComparison.OrdinalIgnoreCase) || i.Priority.Equals("Critical", StringComparison.OrdinalIgnoreCase));
        var old = sprintIssues.Where(i => i.AgeDays >= 10 && !i.Status.Equals("Done", StringComparison.OrdinalIgnoreCase)).Select(i => i.Key).ToArray();

        if (blocked > 0) risks.Add($"{blocked} issue(s) bloqueadas requieren desbloqueo.");
        if (high > 2) risks.Add($"Hay {high} issues de prioridad alta/crítica en el sprint.");
        if (old.Length > 0) risks.Add($"Issues antiguas sin cerrar: {string.Join(", ", old)}.");

        return new JiraSprintSummary(
            projectKey,
            currentSprint,
            sprintIssues.Length,
            sprintIssues.Count(i => i.Status.Equals("Done", StringComparison.OrdinalIgnoreCase)),
            sprintIssues.Count(i => i.Status.Equals("In Progress", StringComparison.OrdinalIgnoreCase)),
            blocked,
            high,
            risks.ToArray());
    }

    private async Task<IReadOnlyList<JiraIssue>> LoadAsync(CancellationToken cancellationToken)
    {
        if (_cache is not null) return _cache;
        var path = _options.MockDataPath ?? Path.Combine(AppContext.BaseDirectory, "jira-mock-data.json");
        if (!Path.IsPathRooted(path)) path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, path));
        if (!File.Exists(path))
        {
            var fallback = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../samples/jira-mock-data.json"));
            path = File.Exists(fallback) ? fallback : path;
        }

        await using var stream = File.OpenRead(path);
        _cache = await JsonSerializer.DeserializeAsync<IReadOnlyList<JiraIssue>>(stream, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }, cancellationToken) ?? [];
        return _cache;
    }
}
