namespace EnterpriseAgentAccelerator.Shared.Contracts;

public sealed record AgentRequest(
    string Question,
    string ProjectKey = "KM",
    string? UserName = null,
    bool IncludeRawToolData = false);
