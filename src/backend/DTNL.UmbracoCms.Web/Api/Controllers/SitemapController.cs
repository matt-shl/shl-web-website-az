using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using Asp.Versioning;
using DTNL.UmbracoCms.Web.Helpers;
using DTNL.UmbracoCms.Web.Infrastructure.Configuration.Options;
using DTNL.UmbracoCms.Web.Services;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Web;

namespace DTNL.UmbracoCms.Web.Api.Controllers;

/// <summary>
/// Represents the sitemap controller.
/// </summary>
[ApiVersionNeutral]
[Route("[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
public sealed class SitemapController : ApiControllerBase, IDisposable
{
    private readonly ISitemapService _sitemapService;
    private readonly IOptionsMonitor<ApplicationOptions> _applicationOptions;
    private UmbracoContextReference _umbracoContextReference;

    public SitemapController(
        ISitemapService sitemapService,
        IUmbracoContextFactory umbracoContextFactory,
        IOptionsMonitor<ApplicationOptions> applicationOptions)
    {
        _sitemapService = sitemapService;
        _applicationOptions = applicationOptions;

        // Using IUmbracoContextFactory since the umbraco context is not available
        // This only happens because our routes have file extensions
        _umbracoContextReference = umbracoContextFactory.EnsureUmbracoContext();
    }

    /// <summary>
    /// Generates the website sitemap.
    /// </summary>
    /// <returns>The sitemap or sitemap index of the website.</returns>
    /// <remarks>This is generated based on the umbraco domains that match the current request authority and culture.</remarks>
    [HttpGet]
    [Route("~/sitemap.xml")]
    [Produces(MediaTypeNames.Text.Xml)]
    public IActionResult Index()
    {
        Uri currentUri = new(Request.GetDisplayUrl());
        if (!_applicationOptions.CurrentValue.IsCrawlableUrl(currentUri))
        {
            return _sitemapService.GenerateEmptySitemap();
        }

        List<(DomainAndUri Domain, IPublishedContent RootNode)> domains = _sitemapService.GetDomainsForHost(currentUri);

        // If there is only one domain matching the current uri return the sitemap directly
        // Otherwise return a sitemap index
        return domains.Count switch
        {
            0 => _sitemapService.GenerateSitemap(domains, _umbracoContextReference.UmbracoContext.Domains?.DefaultCulture),
            1 => _sitemapService.GenerateSitemap(domains, domains.First().Domain.Culture),
            _ => _sitemapService.GenerateSitemapIndex(domains, culture => Url.Action("CultureSpecific", "Sitemap", new { Culture = culture }, Request.Scheme) ?? ""),
        };
    }

    [HttpGet]
    [Route("{culture}.xml")]
    [Produces(MediaTypeNames.Text.Xml)]
    public IActionResult CultureSpecific([FromRoute, Required] string culture)
    {
        Uri currentUri = new(Request.GetDisplayUrl());
        if (!_applicationOptions.CurrentValue.IsCrawlableUrl(currentUri))
        {
            return NotFound();
        }

        List<(DomainAndUri Domain, IPublishedContent RootNode)> domains = _sitemapService.GetDomainsForHost(currentUri);

        return _sitemapService.GenerateSitemap(domains, culture);
    }

    public void Dispose()
    {
        _umbracoContextReference?.Dispose();
        _umbracoContextReference = null!;
    }
}
