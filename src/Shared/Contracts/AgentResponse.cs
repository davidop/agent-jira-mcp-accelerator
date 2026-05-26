namespace EnterpriseAgentAccelerator.Shared.Contracts;

public sealed record AgentResponse(
    string Answer,
    string[] ToolsUsed,
    object? RawToolData = null,
    string[] FollowUpQuestions = null!)
{
    public string[] FollowUpQuestions { get; init; } = FollowUpQuestions ?? [];
}
