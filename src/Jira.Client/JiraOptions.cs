namespace EnterpriseAgentAccelerator.Jira.Client;

public sealed class JiraOptions
{
    public string Mode { get; set; } = "Mock";
    public string? MockDataPath { get; set; }
    public string? BaseUrl { get; set; }
    public string? Email { get; set; }
    public string? ApiToken { get; set; }
    public int MaxResultsPerPage { get; set; } = 50;
    public int MaxPages { get; set; } = 20;
    public int RetryCount { get; set; } = 3;
    public int InitialRetryDelayMs { get; set; } = 500;
    public int RequestTimeoutSeconds { get; set; } = 30;
    public string? OAuthClientId { get; set; }
    public string? OAuthAudience { get; set; }
}
