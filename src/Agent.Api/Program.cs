using EnterpriseAgentAccelerator.Agent.Api.Services;
using EnterpriseAgentAccelerator.Jira.Client;
using EnterpriseAgentAccelerator.Shared.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddJiraClient(builder.Configuration);
builder.Services.AddScoped<DemoAgentService>();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "Agent.Api" }));

app.MapPost("/api/agent/ask", async (AgentRequest request, DemoAgentService agent, CancellationToken ct) =>
{
    var response = await agent.AnswerAsync(request, ct);
    return Results.Ok(response);
});

app.Run();
