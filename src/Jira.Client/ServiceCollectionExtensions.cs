using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EnterpriseAgentAccelerator.Jira.Client;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJiraClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JiraOptions>(configuration.GetSection("Jira"));
        var mode = configuration.GetValue<string>("Jira:Mode") ?? "Mock";
        if (mode.Equals("Cloud", StringComparison.OrdinalIgnoreCase))
        {
            services.AddHttpClient<IJiraReader, JiraCloudReader>();
        }
        else
        {
            services.AddSingleton<IJiraReader, MockJiraReader>();
        }
        return services;
    }
}
