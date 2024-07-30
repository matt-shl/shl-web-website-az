using DTNL.UmbracoCms.Web.Infrastructure.Configuration.Options;
using DTNL.UmbracoCms.Web.Services.Assets;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.FileProviders;

namespace DTNL.UmbracoCms.Web.Helpers;

/// <summary>
/// Helper class for development setup or debugging functionalities.
/// </summary>
public static class DevelopmentHelpers
{
    public static void ConfigureDevelopmentAssetsFallback(this IServiceCollection services, IWebHostEnvironment webHostEnvironment, DevelopmentOptions developmentOptions)
    {
        List<IFileProvider>? fileProviders = [webHostEnvironment.WebRootFileProvider];
        fileProviders.AddRange(
            developmentOptions.AssetsFallbackDirectories
                .Select(directory => Path.Combine(Directory.GetCurrentDirectory(), directory))
                .Where(Directory.Exists)
                .Select(path => new PhysicalFileProvider(path)));

        webHostEnvironment.WebRootFileProvider = new CompositeFileProvider(fileProviders);

        if (developmentOptions.AssetsFallbackUri != null)
        {
            services.Decorate<IAssetsProvider, DevelopmentFallbackAssetsProvider>();
        }
    }

    /// <summary>
    /// Adds the required middlewares to fallback.
    /// </summary>
    public static IApplicationBuilder UseDevelopmentAssetsFallback(this IApplicationBuilder app, DevelopmentOptions developmentOptions)
    {
        if (developmentOptions.AssetsFallbackUri == null)
        {
            return app;
        }

        // Fallback to a redirect (e.g Test Environment)
        foreach (string directory in developmentOptions.AssetsSubdirectories)
        {
            app.UseRewriter(new RewriteOptions().AddRedirect($"^{directory}/.*", developmentOptions.AssetsFallbackUri + "$0"));
        }

        return app;
    }
}
