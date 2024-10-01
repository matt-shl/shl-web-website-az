using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Umbraco.Cms.Core.Sync;

namespace DTNL.UmbracoCms.Web.Infrastructure.Configuration.Options;

public class ApplicationOptions : IValidatableObject
{
    public enum HostingEnvironmentType
    {
        Default = 0,
        AzureWebApp = 1,
    }

    public CacheOptions Cache { get; protected set; } = new();

    public CorsPolicy Cors { get; protected set; } = new();

    public DevelopmentOptions Development { get; protected set; } = new();

    public BackgroundJobsOptions BackgroundJobs { get; protected set; } = new();

    public HostingEnvironmentType HostingEnvironment { get; set; } = HostingEnvironmentType.Default;

    public ServerRole ServerRole { get; set; }

    public List<string> CrawlableDomains { get; set; } = [];

    public required bool EnableCriticalCss { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ServerRole == ServerRole.Unknown)
        {
            yield return new ValidationResult("ServerRole must be set.", [nameof(ServerRole)]);
        }
    }
}
