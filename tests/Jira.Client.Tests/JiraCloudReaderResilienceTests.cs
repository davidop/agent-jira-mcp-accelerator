using System.Net;
using System.Net.Http.Headers;
using System.Text;
using EnterpriseAgentAccelerator.Jira.Client;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace Jira.Client.Tests;

public sealed class JiraCloudReaderResilienceTests
{
    [Fact]
    public async Task Normalize_Status_Maps_Common_Jira_Statuses_Deterministically()
    {
        var reader = CreateReader(
            SequenceHandler.FromJsonResponses(
                SearchResponse(
                    total: 4,
                    new IssueSpec("KM-1", "resolved"),
                    new IssueSpec("KM-2", "in development"),
                    new IssueSpec("KM-3", "Impeded"),
                    new IssueSpec("KM-4", "selected for development"))));

        var issues = await reader.GetProjectIssuesAsync("KM");

        Assert.Collection(
            issues,
            issue => Assert.Equal("Done", issue.Status),
            issue => Assert.Equal("In Progress", issue.Status),
            issue => Assert.Equal("Blocked", issue.Status),
            issue => Assert.Equal("To Do", issue.Status));
    }

    [Fact]
    public async Task Paging_Stops_At_MaxPages_Even_When_Total_Is_Higher()
    {
        var handler = SequenceHandler.FromJsonResponses(
            SearchResponse(
                total: 5,
                new IssueSpec("KM-10", "To Do"),
                new IssueSpec("KM-11", "To Do")),
            SearchResponse(
                total: 5,
                new IssueSpec("KM-12", "To Do"),
                new IssueSpec("KM-13", "To Do")));

        var reader = CreateReader(handler, new JiraOptions
        {
            BaseUrl = "https://example.local",
            Email = "test@example.com",
            ApiToken = "token",
            MaxResultsPerPage = 2,
            MaxPages = 2,
            RetryCount = 0,
            InitialRetryDelayMs = 1
        });

        var issues = await reader.GetProjectIssuesAsync("KM");

        Assert.Equal(4, issues.Count);
        Assert.Equal(2, handler.RequestCount);
    }

    [Fact]
    public async Task Retry_Respects_RetryAfter_Then_Succeeds()
    {
        var tooManyRequests = new HttpResponseMessage(HttpStatusCode.TooManyRequests)
        {
            Content = new StringContent("{\"error\":\"rate_limited\"}", Encoding.UTF8, "application/json")
        };
        tooManyRequests.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromSeconds(1));

        var handler = SequenceHandler.FromResponses(
            tooManyRequests,
            JsonResponse(SearchResponse(total: 1, new IssueSpec("KM-20", "Blocked"))));

        var reader = CreateReader(handler, new JiraOptions
        {
            BaseUrl = "https://example.local",
            Email = "test@example.com",
            ApiToken = "token",
            MaxResultsPerPage = 50,
            MaxPages = 2,
            RetryCount = 1,
            InitialRetryDelayMs = 1
        });

        var started = DateTimeOffset.UtcNow;
        var issues = await reader.GetBlockedIssuesAsync("KM");
        var elapsed = DateTimeOffset.UtcNow - started;

        Assert.Single(issues);
        Assert.True(elapsed >= TimeSpan.FromMilliseconds(900), "Expected retry delay from Retry-After header.");
        Assert.Equal(2, handler.RequestCount);
    }

    private static JiraCloudReader CreateReader(SequenceHandler handler, JiraOptions? options = null)
    {
        var resolved = options ?? new JiraOptions
        {
            BaseUrl = "https://example.local",
            Email = "test@example.com",
            ApiToken = "token",
            MaxResultsPerPage = 50,
            MaxPages = 2,
            RetryCount = 1,
            InitialRetryDelayMs = 1
        };

        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri(resolved.BaseUrl!)
        };
        return new JiraCloudReader(client, Options.Create(resolved), NullLogger<JiraCloudReader>.Instance);
    }

    private static HttpResponseMessage JsonResponse(string json) => new(HttpStatusCode.OK)
    {
        Content = new StringContent(json, Encoding.UTF8, "application/json")
    };

    private static string SearchResponse(int total, params IssueSpec[] issues)
    {
        var issueJson = string.Join(",", issues.Select(ToJson));
        return "{" +
               "\"startAt\":0," +
               "\"maxResults\":50," +
               $"\"total\":{total}," +
               "\"issues\":[" + issueJson + "]" +
               "}";
    }

    private static string ToJson(IssueSpec issue) => "{" +
        $"\"key\":\"{issue.Key}\"," +
        "\"fields\":{" +
        "\"summary\":\"sample\"," +
        $"\"status\":{{\"name\":\"{issue.Status}\"}}," +
        "\"priority\":{\"name\":\"High\"}," +
        "\"assignee\":{\"displayName\":\"Tester\"}," +
        "\"labels\":[]," +
        "\"created\":\"2026-05-20T00:00:00Z\"," +
        "\"updated\":\"2026-05-20T00:00:00Z\"" +
        "}" +
        "}";

    private sealed record IssueSpec(string Key, string Status);

    private sealed class SequenceHandler : HttpMessageHandler
    {
        private readonly Queue<Func<HttpResponseMessage>> _responses;

        public int RequestCount { get; private set; }

        private SequenceHandler(IEnumerable<Func<HttpResponseMessage>> responses)
        {
            _responses = new Queue<Func<HttpResponseMessage>>(responses);
        }

        public static SequenceHandler FromJsonResponses(params string[] payloads) => new(payloads.Select(payload => (Func<HttpResponseMessage>)(() => JsonResponse(payload))));

        public static SequenceHandler FromResponses(params HttpResponseMessage[] responses)
        {
            var factories = responses.Select(response =>
            {
                var status = response.StatusCode;
                var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/json";
                var retryAfter = response.Headers.RetryAfter;
                return new Func<HttpResponseMessage>(() =>
                {
                    var clone = new HttpResponseMessage(status)
                    {
                        Content = new StringContent(content, Encoding.UTF8, contentType)
                    };
                    if (retryAfter is not null)
                    {
                        clone.Headers.RetryAfter = retryAfter;
                    }

                    return clone;
                });
            });

            return new SequenceHandler(factories);
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            RequestCount++;
            if (_responses.Count == 0)
            {
                throw new InvalidOperationException("No more fake responses configured.");
            }

            return Task.FromResult(_responses.Dequeue().Invoke());
        }
    }
}
