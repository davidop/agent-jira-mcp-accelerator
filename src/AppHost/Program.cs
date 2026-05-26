if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ASPNETCORE_URLS")))
{
    Environment.SetEnvironmentVariable("ASPNETCORE_URLS", "http://127.0.0.1:18888");
}

if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ASPIRE_DASHBOARD_OTLP_ENDPOINT_URL")) &&
    string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ASPIRE_DASHBOARD_OTLP_HTTP_ENDPOINT_URL")))
{
    Environment.SetEnvironmentVariable("ASPIRE_DASHBOARD_OTLP_ENDPOINT_URL", "http://127.0.0.1:4317");
}

if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ASPIRE_ALLOW_UNSECURED_TRANSPORT")))
{
    Environment.SetEnvironmentVariable("ASPIRE_ALLOW_UNSECURED_TRANSPORT", "true");
}

var builder = DistributedApplication.CreateBuilder(args);

var mcp = builder.AddProject("jira-mcp-server", "../Jira.McpServer/Jira.McpServer.csproj")
    .WithExternalHttpEndpoints();

var api = builder.AddProject("agent-api", "../Agent.Api/Agent.Api.csproj")
    .WithReference(mcp)
    .WithExternalHttpEndpoints();

builder.AddProject("web", "../Web/Web.csproj")
    .WithReference(api)
    .WithExternalHttpEndpoints();

builder.Build().Run();
