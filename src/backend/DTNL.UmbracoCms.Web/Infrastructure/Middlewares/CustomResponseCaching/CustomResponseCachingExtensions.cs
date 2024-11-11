using Microsoft.AspNetCore.ResponseCaching;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DTNL.UmbracoCms.Web.Infrastructure.Middlewares.CustomResponseCaching;

/// <summary>
/// Extension methods for the <see cref="CustomResponseCachingMiddleware"/>.
/// </summary>
public static class CustomResponseCachingExtensions
{
    /// <summary>
    /// Add custom response caching services.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> for adding services.</param>
    public static IServiceCollection AddCustomResponseCaching(this IServiceCollection services)
    {
        return AddCustomResponseCaching(services, _ => { });
    }

    /// <summary>
    /// Add custom response caching services and configure the related options.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> for adding services.</param>
    /// <param name="configureOptions">A delegate to configure the <see cref="ResponseCachingOptions"/>.</param>
    public static IServiceCollection AddCustomResponseCaching(this IServiceCollection services, Action<ResponseCachingOptions> configureOptions)
    {
        services.AddResponseCaching(configureOptions);
        services.TryAddSingleton<CustomResponseCachingMemoryCacheFactory>();

        return services;
    }

    /// <summary>
    /// Adds the <see cref="CustomResponseCachingMiddleware"/> for caching HTTP responses.
    /// </summary>
    /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
    public static IApplicationBuilder UseCustomResponseCaching(this IApplicationBuilder app)
    {
        return app.UseMiddleware<CustomResponseCachingMiddleware>();
    }
}
