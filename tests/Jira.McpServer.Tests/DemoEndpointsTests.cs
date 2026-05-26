using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Jira.McpServer.Tests;

public sealed class DemoEndpointsTests : IClassFixture<DemoEndpointsTests.McpServerFactory>
{
    private readonly HttpClient _client;

    public DemoEndpointsTests(McpServerFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Blocked_Endpoint_Returns_ReadOnly_Blocked_Items()
    {
        var response = await _client.GetAsync("/demo/projects/KM/blocked");
        response.EnsureSuccessStatusCode();

        var items = await response.Content.ReadFromJsonAsync<List<BlockedIssueDto>>();

        Assert.NotNull(items);
        Assert.NotEmpty(items!);
        Assert.All(items!, issue => Assert.True(issue.Blocked || issue.Status.Equals("Blocked", StringComparison.OrdinalIgnoreCase)));
    }

    [Fact]
    public async Task Sprint_Summary_Endpoint_Returns_Deterministic_Shape()
    {
        var response = await _client.GetAsync("/demo/projects/KM/sprint-summary");
        response.EnsureSuccessStatusCode();

        var summary = await response.Content.ReadFromJsonAsync<SprintSummaryDto>();

        Assert.NotNull(summary);
        Assert.Equal("KM", summary!.ProjectKey);
        Assert.True(summary.Total >= 0);
    }

    [Fact]
    public async Task Demo_Endpoints_Do_Not_Expose_Write_Operations()
    {
        var postBlocked = await _client.PostAsync("/demo/projects/KM/blocked", new StringContent("{}"));
        var postSprint = await _client.PostAsync("/demo/projects/KM/sprint-summary", new StringContent("{}"));

        Assert.Contains(postBlocked.StatusCode, new[] { HttpStatusCode.MethodNotAllowed, HttpStatusCode.NotFound });
        Assert.Contains(postSprint.StatusCode, new[] { HttpStatusCode.MethodNotAllowed, HttpStatusCode.NotFound });
    }

    private static string FindFixturePath()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            var candidate = Path.Combine(current.FullName, "tests", "Fixtures", "jira-mock-stable.json");
            if (File.Exists(candidate))
            {
                return candidate;
            }

            current = current.Parent;
        }

        throw new FileNotFoundException("Could not locate tests/Fixtures/jira-mock-stable.json");
    }

    public sealed class McpServerFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureTestServices(_ => { });
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Jira:Mode"] = "Mock",
                    ["Jira:MockDataPath"] = FindFixturePath()
                });
            });
        }
    }

    public sealed record BlockedIssueDto(string Key, string Status, bool Blocked);

    public sealed record SprintSummaryDto(string ProjectKey, string Sprint, int Total, int Done, int InProgress, int Blocked, int HighPriority, string[] Risks);
}
