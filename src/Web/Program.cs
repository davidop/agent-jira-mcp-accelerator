using EnterpriseAgentAccelerator.ServiceDefaults;
using EnterpriseAgentAccelerator.Shared.Contracts;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

var agentApiBaseUrl =
    builder.Configuration.GetValue<string>("services:agent-api:https:0") ??
    builder.Configuration.GetValue<string>("services:agent-api:http:0") ??
    builder.Configuration.GetValue<string>("AgentApi:BaseUrl") ??
    "https://localhost:7041";

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient("agent", client =>
{
    client.BaseAddress = new Uri(agentApiBaseUrl);
});

var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.MapDefaultEndpoints();
app.Run();
