using System.Reflection;
using DTNL.UmbracoCms.Web.Infrastructure.Configuration.Options;
using DTNL.UmbracoCms.Web.Infrastructure.DatabaseMigrations;
using DTNL.UmbracoCms.Web.Infrastructure.DataProtection;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Caching.SqlServer;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Serilog;
using Umbraco.Cms.Core.Configuration;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Sync;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Website.Controllers;

namespace DTNL.UmbracoCms.Web.Infrastructure.Configuration;

public static class UmbracoConfigurationExtensions
{
    /// <summary>
    /// Configures the necessary Umbraco settings for use in an Azure WebApp.
    /// </summary>
    /// <remarks>Needs to run before the Umbraco Builder.</remarks>
    public static IServiceCollection ConfigureUmbracoForAzureWebApp(this IServiceCollection services, IConfiguration config, ServerRole serverRole)
    {
        // This needs to run before the UmbracoBuilder and needs to be configured directly as Umbraco doesn't use IOptions for it
        config["Umbraco:CMS:Hosting:LocalTempStorageLocation"] = nameof(LocalTempStorage.EnvironmentTemp);

        services.Configure<GlobalSettings>(options => options.MainDomLock = "FileSystemMainDomLock");
        services.Configure<IndexCreatorSettings>(options =>
            options.LuceneDirectoryFactory = serverRole switch
            {
                ServerRole.SchedulingPublisher or ServerRole.Single => LuceneDirectoryFactory.SyncedTempFileSystemDirectoryFactory,
                ServerRole.Subscriber => LuceneDirectoryFactory.TempFileSystemDirectoryFactory,
                ServerRole.Unknown => options.LuceneDirectoryFactory,
                _ => options.LuceneDirectoryFactory,
            }
        );

        return services;
    }

    /// <summary>
    /// Configures the required services for use in a Load Balanced scenario.
    /// </summary>
    public static IUmbracoBuilder AddLoadBalancingServices(this IUmbracoBuilder umbracoBuilder, IWebHostEnvironment environment, ServerRole serverRole)
    {
        if (serverRole == ServerRole.Single)
        {
            return umbracoBuilder;
        }

        // SolutionName is used to ensure the Application Discriminator is different for different solutions.
        string? solutionName = typeof(Startup).Assembly
            .GetCustomAttributes<AssemblyMetadataAttribute>()
            .FirstOrDefault(metadata => metadata.Key == "SolutionName")
            ?.Value;

        if (solutionName is null)
        {
            // Using static logger as the DI container isn't ready yet.
            Log.Warning("Unable to retrieve SolutionName from Assembly Metadata. Either correct this or set the value to an unique name manually.");
            solutionName = "Unknown";
        }

        umbracoBuilder.Services
            .AddDataProtectionSqlRepository()
            .Configure<DataProtectionOptions>(options =>
            {
                // This should ensure that even if databases are erroneously copied any secure payloads aren't interchangeable or shared.
                options.ApplicationDiscriminator = $"{solutionName}-{environment.ApplicationName}-{environment.EnvironmentName}";
            })
            .AddDistributedSqlServerCache(_ => { })
            .AddOptions<SqlServerCacheOptions>().Configure((Action<SqlServerCacheOptions, IOptions<ConnectionStrings>>) ((options, connectionStrings) =>
             {
                 options.SchemaName = "dbo";
                 options.TableName = DistributedSqlServerCacheMigration.TableName;
                 options.ConnectionString = connectionStrings.Value.ConnectionString;
             }));

        return umbracoBuilder;
    }

    /// <summary>
    /// Registers Umbraco Azure Blob filesystem when the hosting environment is set to AzureWebApp.
    /// </summary>
    public static IUmbracoBuilder AddAzureBlobFileSystem(this IUmbracoBuilder umbracoBuilder, ApplicationOptions.HostingEnvironmentType hostingEnvironment)
    {
        if (hostingEnvironment == ApplicationOptions.HostingEnvironmentType.AzureWebApp)
        {
            umbracoBuilder
                .AddAzureBlobMediaFileSystem()
                .AddAzureBlobImageSharpCache();
        }

        return umbracoBuilder;
    }

    /// <summary>
    /// Sets the default umbraco render controller.
    /// </summary>
    public static IUmbracoBuilder SetDefaultRenderController<T>(this IUmbracoBuilder umbracoBuilder)
        where T : IRenderController
    {
        umbracoBuilder.Services.Configure<UmbracoRenderingDefaultsOptions>(options => options.DefaultControllerType = typeof(T));

        return umbracoBuilder;
    }

    public static IServiceCollection ConfigureStaticAssets(this IServiceCollection services)
    {
        services.Configure<StaticFileOptions>(options =>
        {
            options.ContentTypeProvider = new FileExtensionContentTypeProvider
            {
                // Extra Content-Type mappings
                Mappings =
                {
                    [".less"] = "text/css",
                },
            };

            options.OnPrepareResponse = context =>
            {
                // Add Caching Headers
                ResponseHeaders headers = context.Context.Response.GetTypedHeaders();
                headers.CacheControl = new CacheControlHeaderValue
                {
                    Public = true,
                    MaxAge = TimeSpan.FromHours(24),
                };
            };
        });

        return services;
    }
}
