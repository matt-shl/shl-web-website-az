using DTNL.UmbracoCms.Web.Infrastructure.Configuration;

namespace DTNL.UmbracoCms.Web.Infrastructure.ApiClients.Ats.Configuration;

public static class ConfigurationExtensions
{
    public static IServiceCollection ConfigureAtsApiClient(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<AtsApiClientOptions>(configuration)
            .ValidateDataAnnotations();

        services.AddTransient<IAtsApiClient, AtsApiClient>();

        return services;
    }
}
