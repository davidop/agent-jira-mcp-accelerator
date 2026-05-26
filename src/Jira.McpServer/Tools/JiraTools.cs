using System.ComponentModel;
using EnterpriseAgentAccelerator.Jira.Client;
using EnterpriseAgentAccelerator.Jira.Client.Models;
using ModelContextProtocol.Server;

namespace EnterpriseAgentAccelerator.Jira.McpServer.Tools;

[McpServerToolType]
public sealed class JiraTools
{
    private readonly IJiraReader _jira;

    public JiraTools(IJiraReader jira) => _jira = jira;

    [McpServerTool]
    [Description("Gets Jira issues for a project key, for example KM.")]
    public Task<IReadOnlyList<JiraIssue>> GetProjectIssues(
        [Description("Jira project key, for example KM.")] string projectKey,
        CancellationToken cancellationToken) => _jira.GetProjectIssuesAsync(projectKey, cancellationToken);

    [McpServerTool]
    [Description("Gets blocked Jira issues for a project key.")]
    public Task<IReadOnlyList<JiraIssue>> GetBlockedIssues(
        [Description("Jira project key, for example KM.")] string projectKey,
        CancellationToken cancellationToken) => _jira.GetBlockedIssuesAsync(projectKey, cancellationToken);

    [McpServerTool]
    [Description("Gets Jira issues assigned to a user by display name or email fragment.")]
    public Task<IReadOnlyList<JiraIssue>> GetIssuesByAssignee(
        [Description("Assignee display name or email fragment.")] string assignee,
        CancellationToken cancellationToken) => _jira.GetIssuesByAssigneeAsync(assignee, cancellationToken);

    [McpServerTool]
    [Description("Gets issues linked to a Jira epic.")]
    public Task<IReadOnlyList<JiraIssue>> GetIssuesByEpic(
        [Description("Epic key, for example KM-10.")] string epicKey,
        CancellationToken cancellationToken) => _jira.GetIssuesByEpicAsync(epicKey, cancellationToken);

    [McpServerTool]
    [Description("Gets a sprint delivery summary including progress, blocked work and risks.")]
    public Task<JiraSprintSummary> GetSprintSummary(
        [Description("Jira project key, for example KM.")] string projectKey,
        [Description("Optional sprint name. If empty, current/open sprint is used.")] string? sprint,
        CancellationToken cancellationToken) => _jira.GetSprintSummaryAsync(projectKey, sprint, cancellationToken);
}
