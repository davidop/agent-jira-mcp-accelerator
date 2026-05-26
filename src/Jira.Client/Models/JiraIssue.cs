namespace EnterpriseAgentAccelerator.Jira.Client.Models;

public sealed record JiraIssue(
    string Key,
    string ProjectKey,
    string Summary,
    string Status,
    string Priority,
    string Assignee,
    string EpicKey,
    string EpicName,
    string Sprint,
    bool Blocked,
    int AgeDays,
    DateTimeOffset UpdatedAt,
    string[] Labels,
    string? BlockReason = null);
