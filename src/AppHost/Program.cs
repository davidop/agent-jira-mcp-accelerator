var builder = DistributedApplication.CreateBuilder(args);

var mcp = builder.AddProject<Projects.Jira_McpServer>("jira-mcp-server");
var api = builder.AddProject<Projects.Agent_Api>("agent-api")
    .WithReference(mcp);

builder.AddProject<Projects.Web>("web")
    .WithReference(api);

builder.Build().Run();
