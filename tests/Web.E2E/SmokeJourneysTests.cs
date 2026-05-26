using System.Net.Http.Json;
using EnterpriseAgentAccelerator.Shared.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Web.E2E.Tests;

public sealed class SmokeJourneysTests : IClassFixture<SmokeJourneysTests.SmokeApiFactory>
{
    private readonly HttpClient _client;

    public SmokeJourneysTests(SmokeApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Smoke_Blocked_Issues_Journey_Returns_Blocked_Tool_Hint()
    {
        var response = await _client.PostAsJsonAsync("/api/agent/ask", new AgentRequest("Show blocked issues for KM", "KM", "David", false));
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<AgentResponse>();

        Assert.NotNull(payload);
        Assert.Contains("get_blocked_issues", payload!.ToolsUsed);
    }

    [Fact]
    public async Task Smoke_Sprint_Summary_Journey_Returns_Sprint_Tool_Hint()
    {
        var response = await _client.PostAsJsonAsync("/api/agent/ask", new AgentRequest("Summarize current sprint for steering committee", "KM", "David", false));
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<AgentResponse>();

        Assert.NotNull(payload);
        Assert.Contains("get_sprint_summary", payload!.ToolsUsed);
    }

    [Fact]
    public async Task Smoke_Executive_Report_Journey_Returns_Sprint_Tool_Hint()
    {
        var response = await _client.PostAsJsonAsync("/api/agent/ask", new AgentRequest("Generate executive report with risks and next steps", "KM", "David", false));
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<AgentResponse>();

        Assert.NotNull(payload);
        Assert.Contains("get_sprint_summary", payload!.ToolsUsed);
        Assert.False(string.IsNullOrWhiteSpace(payload.Answer));
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

    public sealed class SmokeApiFactory : WebApplicationFactory<Program>
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
}
