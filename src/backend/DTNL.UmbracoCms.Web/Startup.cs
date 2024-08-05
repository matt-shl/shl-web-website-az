using System.Text.Json.Serialization;
using Asp.Versioning;
using DTNL.UmbracoCms.Web.Api;
using DTNL.UmbracoCms.Web.Controllers;
using DTNL.UmbracoCms.Web.Helpers;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using DTNL.UmbracoCms.Web.Infrastructure;
using DTNL.UmbracoCms.Web.Infrastructure.Configuration;
using DTNL.UmbracoCms.Web.Infrastructure.Configuration.Options;
using DTNL.UmbracoCms.Web.Infrastructure.ContentFinders;
using DTNL.UmbracoCms.Web.Infrastructure.Middlewares.CustomResponseCaching;
using DTNL.UmbracoCms.Web.Infrastructure.NotificationHandlers;
using DTNL.UmbracoCms.Web.Services;
using DTNL.UmbracoCms.Web.Services.Assets;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Options;
using Scrutor;
using SimpleMvcSitemap;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Sync;
using Umbraco.Cms.Infrastructure.DependencyInjection;
using Umbraco.Cms.Web.Common.Routing;

namespace DTNL.UmbracoCms.Web;

/// <summary>
/// Responsible for app startup and configuration.
/// </summary>
public static class Startup
{
    /// <summary>
    /// Configures the <see cref="IWebHost"/>.
    /// </summary>
    public static void ConfigureWebHost(this WebApplicationBuilder builder)
    {
        builder.WebHost.ConfigureKestrel(options =>
        {
            // Remove Kestrel Server header
            options.AddServerHeader = false;

            // MaxRequestBodySize is not configured here,
            // as Umbraco already sets it based on Umbraco:CMS:Runtime:MaxRequestLength
        });

        if (builder.Configuration.GetRuntimeMode() != RuntimeMode.Production)
        {
            builder.WebHost.UseStaticWebAssets();
        }
    }

    /// <summary>
    /// Configures the app <see cref="IConfiguration"/>.
    /// </summary>
    public static void ConfigureAppConfiguration(this ConfigurationManager configuration, IWebHostEnvironment environment)
    {
        // This allows us to load all of the debug settings when running under a different environment
        string? appSettingsOverride = Environment.GetEnvironmentVariable("APPSETTINGS_OVERRIDE");

        if (environment.IsDevelopment() && appSettingsOverride is null or "")
        {
            appSettingsOverride = "Debug";
        }

        configuration.AddJsonFile($"appsettings.Overrides.{appSettingsOverride}.json", optional: true, reloadOnChange: true);

        /*
         * Extra configuration sources can be added here
         */
    }

    /// <summary>
    /// Configures the services on the <see cref="IServiceCollection"/> container.
    /// </summary>
    public static void ConfigureServices(this IServiceCollection services, IWebHostEnvironment environment, IConfigurationRoot configuration)
    {
        // Configure Application Options
        IConfigurationSection applicationOptionsConfigSection = configuration.GetSection<ApplicationOptions>();
        services.AddOptions<ApplicationOptions>(configuration);
        services.AddOptions<DevelopmentOptions>(applicationOptionsConfigSection);
        services.AddOptions<CacheOptions>(applicationOptionsConfigSection);

        ApplicationOptions applicationOptions = applicationOptionsConfigSection.Get<ApplicationOptions>() ?? throw new InvalidOperationException("Unable to bind ApplicationOptions");

        // Configure Application Insights
        services.AddApplicationInsightsTelemetry(opt =>
        {
            // Required for the Serilog AI Sink (As Umbraco prevents us from configuring it through code)
            opt.EnableActiveTelemetryConfigurationSetup = true;
        });

        // Caching Services
        services.AddCustomResponseCaching();
        services.AddSingleton<ICacheManager, CacheManager>();

        // Assets Services
        services.ConfigureStaticAssets();
        services.AddTransient<IAssetsProvider, FileSystemAssetsProvider>();
        services.ConfigureDevelopmentAssetsFallback(environment, applicationOptions.Development);
        services.Decorate<IAssetsProvider, CachedAssetsProvider>();

        // All other application services that use the attributes
        services.Scan(scan => scan
            .FromCallingAssembly()
            .AddClasses(classes => classes.WithAttribute<ServiceDescriptorAttribute>())
            .UsingRegistrationStrategy(RegistrationStrategy.Throw)
            .UsingAttributes()
        );

        // Other 3rd party services
        services.AddTransient<ISitemapProvider, SitemapProvider>();

        // Umbraco Azure Configuration
        if (applicationOptions.HostingEnvironment == ApplicationOptions.HostingEnvironmentType.AzureWebApp)
        {
            services.ConfigureUmbracoForAzureWebApp(configuration, applicationOptions.ServerRole);
        }

        // Umbraco Services
        services.AddUmbraco(environment, configuration)
            .AddBackOffice(builder =>
            {
                builder.AddMvcOptions(options => options.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider()));
                builder.AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
                builder.AddXmlDataContractSerializerFormatters();
                builder.AddRazorOptions(options =>
                {
                    options.ViewLocationFormats.Add("/{0}.cshtml");
                    options.ViewLocationFormats.Add("/Components/{0}.cshtml");
                    options.ViewLocationFormats.Add("/Components/{0}/{0}.cshtml");
                });
                builder.Services.AddApiVersioning(config =>
                {
                    config.DefaultApiVersion = new ApiVersion(1, 0);
                    config.AssumeDefaultVersionWhenUnspecified = true;
                });

                builder.Services.Configure<RouteOptions>(options =>
                {
                    options.LowercaseUrls = true;
                    options.AppendTrailingSlash = false;
                });

                builder.Services.AddCors(options => options.AddDefaultPolicy(applicationOptions.Cors));
            })
            .AddWebsite()
            .AddComposers()
            .AddLoadBalancingServices(environment, applicationOptions.ServerRole)
            .AddAzureBlobFileSystem(applicationOptions.HostingEnvironment)
            .AddNotificationHandler<ContentCacheRefresherNotification, CacheFlushingNotificationHandler>()
            .AddNotificationHandler<MediaCacheRefresherNotification, CacheFlushingNotificationHandler>()
            .AddNotificationAsyncHandler<MediaSavingNotification, DominantColorNotificationHandler>()
            .SetServerRegistrar<ConfigurationServerRoleAccessor>()
            .SetContentLastChanceFinder<LastChanceContentFinder>()
            .SetDefaultRenderController<DefaultRenderController>()
            .Build();
    }

    /// <summary>
    /// Configures the <see cref="WebApplication"/> request pipeline.
    /// </summary>
    public static void ConfigureWebApplication(this WebApplication app)
    {
        ApplicationOptions applicationOptions = app.Services.GetRequiredService<IOptions<ApplicationOptions>>().Value;

        if (applicationOptions.Development.DeveloperExceptionPage)
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler($"{ErrorPagesHelper.ErrorPathPrefix}/500");
        }

        // Security headers
        // SameOrigin iFrames are allowed by default for the Umbraco Preview
        app.UseSecurityHeaders(policies => policies
            .AddDefaultSecurityHeaders()
            .AddContentSecurityPolicy(builder =>
            {
                builder.AddObjectSrc().None();
                builder.AddFormAction().Self();
                builder.AddFrameAncestors().Self();
                builder.AddScriptSrc()
                    .From("https://go.shl-medical.com")
                    .UnsafeInline();
            })
            .AddFrameOptionsSameOrigin()
        );

        // The CORS Middleware should normally run after the EndpointRoutingMiddleware
        // But as we normally only support a default policy and don't need per endpoint policies, we run it sooner.
        // This allows the ResponseCaching middleware to also be called sooner and bypass a lot of middlewares for cached responses.
        app.UseCors();

        // Status Code Pages (e.g pretty 404 for media)
        app.UseStatusCodePagesWithReExecute($"{ErrorPagesHelper.ErrorPathPrefix}/{{0}}");
        app.Use((context, next) =>
        {
            // Ensure we clear the UmbracoRouteValues so the Umbraco routing executes again
            // This is needed because the path may have changed due to an ExceptionHandlerMiddleware or StatusCodeMiddleware re-execution
            context.Features.Set<UmbracoRouteValues>(null);
            context.RequestServices.GetRequiredService<NodeProvider>().Reset();

            return next(context);
        });

        app.UseUmbraco()
            .WithCustomMiddleware(u =>
            {
                u.RunPrePipeline();

                app.UseUmbracoMediaFileProvider();

                app.UseDefaultFiles();

                // Static files
                app.UseStaticFiles();
                app.UseUmbracoPluginsStaticFiles();
                app.UseDevelopmentAssetsFallback(applicationOptions.Development);

                app.UseCustomResponseCaching();

                u.UseUmbracoCoreMiddleware();

                u.RunPreRouting();
                app.UseRouting();
                u.RunPostRouting();

                app.UseWhen(
                    context => context.IsUmbracoPageRequest(),
                    builder => builder.UseRewriter(
                        new RewriteOptions()
                            .AddRedirect("^(.+)/$", "$1", StatusCodes.Status308PermanentRedirect) // Remove trailing slash
                    )
                );

                app.UseAuthentication();
                app.UseAuthorization();

                app.UseRequestLocalization();

                app.UseSession();

                u.RunPostPipeline();

                u.UseBackOffice();
                u.UseWebsite();
            })
            .WithEndpoints(u =>
            {
                if (applicationOptions.ServerRole != ServerRole.Subscriber)
                {
                    u.UseInstallerEndpoints();
                    u.UseBackOfficeEndpoints();
                }

                u.EndpointRouteBuilder.UseRobots();
                u.EndpointRouteBuilder.UseApiFallbackRoute();

                u.UseWebsiteEndpoints();
            });
    }
}
