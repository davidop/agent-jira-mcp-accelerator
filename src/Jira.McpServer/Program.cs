using EnterpriseAgentAccelerator.Jira.Client;
using EnterpriseAgentAccelerator.Jira.McpServer.Tools;
using EnterpriseAgentAccelerator.ServiceDefaults;
using ModelContextProtocol.Server;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddJiraClient(builder.Configuration);
builder.Services.AddSingleton<JiraTools>();

builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "Jira.McpServer" }));
app.MapMcp("/mcp");

// HTTP helper endpoints for demos and Postman. The MCP tools expose the same use cases.
app.MapGet("/demo/projects/{projectKey}/blocked", async (string projectKey, IJiraReader jira, CancellationToken ct) =>
    Results.Ok(await jira.GetBlockedIssuesAsync(projectKey, ct)));

app.MapGet("/demo/projects/{projectKey}/sprint-summary", async (string projectKey, IJiraReader jira, CancellationToken ct) =>
    Results.Ok(await jira.GetSprintSummaryAsync(projectKey, null, ct)));

app.MapDefaultEndpoints();

app.Run();

public partial class Program;
