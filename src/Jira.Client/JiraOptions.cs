namespace EnterpriseAgentAccelerator.Jira.Client;

public sealed class JiraOptions
{
    public string Mode { get; set; } = "Mock";
    public string? MockDataPath { get; set; }
    public string? BaseUrl { get; set; }
    public string? Email { get; set; }
    public string? ApiToken { get; set; }
}
