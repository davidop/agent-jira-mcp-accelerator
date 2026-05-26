namespace EnterpriseAgentAccelerator.Jira.Client.Models;

public sealed record JiraSprintSummary(
    string ProjectKey,
    string Sprint,
    int Total,
    int Done,
    int InProgress,
    int Blocked,
    int HighPriority,
    string[] Risks);
