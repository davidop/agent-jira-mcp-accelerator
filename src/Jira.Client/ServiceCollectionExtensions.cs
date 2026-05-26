using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EnterpriseAgentAccelerator.Jira.Client;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJiraClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JiraOptions>(configuration.GetSection("Jira"));
        var mode = configuration.GetValue<string>("Jira:Mode") ?? "Mock";
        if (mode.Equals("Cloud", StringComparison.OrdinalIgnoreCase))
        {
            var baseUrl = configuration.GetValue<string>("Jira:BaseUrl");
            var email = configuration.GetValue<string>("Jira:Email");
            var apiToken = configuration.GetValue<string>("Jira:ApiToken");
            if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(apiToken))
            {
                throw new InvalidOperationException("Jira Cloud mode requires Jira:BaseUrl, Jira:Email and Jira:ApiToken.");
            }

            services.AddHttpClient<IJiraReader, JiraCloudReader>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<JiraOptions>>().Value;
                var timeout = Math.Max(options.RequestTimeoutSeconds, 5);
                client.Timeout = TimeSpan.FromSeconds(timeout);
            });
        }
        else
        {
            services.AddSingleton<IJiraReader, MockJiraReader>();
        }
        return services;
    }
}
