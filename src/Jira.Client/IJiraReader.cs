using EnterpriseAgentAccelerator.Jira.Client.Models;

namespace EnterpriseAgentAccelerator.Jira.Client;

public interface IJiraReader
{
    Task<IReadOnlyList<JiraIssue>> GetProjectIssuesAsync(string projectKey, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JiraIssue>> GetBlockedIssuesAsync(string projectKey, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JiraIssue>> GetIssuesByAssigneeAsync(string assignee, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JiraIssue>> GetIssuesByEpicAsync(string epicKey, CancellationToken cancellationToken = default);
    Task<JiraSprintSummary> GetSprintSummaryAsync(string projectKey, string? sprint = null, CancellationToken cancellationToken = default);
}
