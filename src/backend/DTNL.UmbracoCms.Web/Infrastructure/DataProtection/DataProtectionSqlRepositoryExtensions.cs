using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Infrastructure.Scoping;

namespace DTNL.UmbracoCms.Web.Infrastructure.DataProtection;

public static class DataProtectionSqlRepositoryExtensions
{
    /// <summary>
    /// Configures the data protection system to persist keys to a Sql Database using <see cref="DataProtectionSqlRepository"/>.
    /// </summary>
    /// <remarks>A custom implementation is used so we don't have a dependency on Entity Framework.</remarks>
    public static IServiceCollection AddDataProtectionSqlRepository(this IServiceCollection services)
    {
        services.AddSingleton<IConfigureOptions<KeyManagementOptions>>(serviceProvider =>
            new ConfigureOptions<KeyManagementOptions>(options =>
            {
                // Use our custom DataProtectionSqlRepository implementation
                options.XmlRepository = new DataProtectionSqlRepository(serviceProvider.GetRequiredService<IScopeProvider>());
            })
        );

        return services;
    }
}
