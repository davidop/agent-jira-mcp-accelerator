using System.Net.Http.Json;
using EnterpriseAgentAccelerator.Shared.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Agent.Api.Tests;

public sealed class AgentAskEndpointTests : IClassFixture<AgentAskEndpointTests.AgentApiFactory>
{
    private readonly HttpClient _client;

    public AgentAskEndpointTests(AgentApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Theory]
    [InlineData("What blocked issues do we have in KM?", "get_blocked_issues")]
    [InlineData("Give me the sprint summary for KM", "get_sprint_summary")]
    [InlineData("Prepare an executive report for KM", "get_sprint_summary")]
    public async Task Ask_Endpoint_Returns_Expected_Tool_Hints_For_Critical_Prompts(string prompt, string expectedTool)
    {
        var response = await _client.PostAsJsonAsync("/api/agent/ask", new AgentRequest(prompt, "KM", "David", false));

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<AgentResponse>();

        Assert.NotNull(payload);
        Assert.False(string.IsNullOrWhiteSpace(payload!.Answer));
        Assert.Contains(expectedTool, payload.ToolsUsed);
        Assert.NotNull(payload.FollowUpQuestions);
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

    public sealed class AgentApiFactory : WebApplicationFactory<Program>
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
