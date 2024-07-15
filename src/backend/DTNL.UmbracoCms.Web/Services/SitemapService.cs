using DTNL.UmbracoCms.Web.Helpers;
using DTNL.UmbracoCms.Web.Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using SimpleMvcSitemap;
using SimpleMvcSitemap.Translations;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Services;

/// <summary>
/// Service for Sitemap related functionality.
/// </summary>
public interface ISitemapService
{
    /// <summary>
    /// Retrieves the list of umbraco domains matching the current uri.
    /// </summary>
    List<(DomainAndUri Domain, IPublishedContent RootNode)> GetDomainsForHost(Uri currentUri);

    /// <summary>
    /// Generates the sitemap index model based on a list of domains and their corresponding root nodes.
    /// </summary>
    ActionResult GenerateSitemapIndex(
        IEnumerable<(DomainAndUri Domain,
        IPublishedContent RootNode)> domains,
        Func<string, string> sitemapCultureUrlMapper
    );

    /// <summary>
    /// Generates the sitemap based on the domains that fits the criteria the best. If no match is found, the first node in umbraco is used.
    /// </summary>
    ActionResult GenerateSitemap(
        List<(DomainAndUri Domain, IPublishedContent RootNode)> domains,
        string? culture);

    /// <summary>
    /// Generates an empty sitemap response.
    /// </summary>
    ActionResult GenerateEmptySitemap();
}

/// <summary>
/// Service for Sitemap related functionality.
/// </summary>
[Transient]
public class SitemapService : ISitemapService
{
    private readonly ISitemapProvider _sitemapProvider;
    private readonly IGlobalizationService _globalizationService;
    private readonly IUmbracoContextAccessor _umbracoContextAccessor;
    private readonly IVariationContextAccessor _variationContextAccessor;

    public SitemapService(ISitemapProvider sitemapProvider, IGlobalizationService globalizationService, IUmbracoContextAccessor umbracoContextAccessor, IVariationContextAccessor variationContextAccessor)
    {
        _sitemapProvider = sitemapProvider;
        _globalizationService = globalizationService;
        _umbracoContextAccessor = umbracoContextAccessor;
        _variationContextAccessor = variationContextAccessor;
    }

    private static ChangeFrequency? GetChangeFrequency(IPublishedContent node)
    {
        return node switch
        {
            PageHome => ChangeFrequency.Monthly,
            _ => ChangeFrequency.Weekly,
        };
    }

    private static decimal GetPriority(IPublishedContent node)
    {
        return node switch
        {
            PageHome => 0.8m,
            _ => 0.6m,
        };
    }

    /// <inheritdoc />
    public List<(DomainAndUri Domain, IPublishedContent RootNode)> GetDomainsForHost(Uri currentUri)
    {
        IUmbracoContext umbracoContext = _umbracoContextAccessor.GetRequiredUmbracoContext();
        IDomainCache? domainCache = umbracoContext.Domains;

        IEnumerable<Domain> allDomains = domainCache?.GetAll(false) ?? [];
        List<(DomainAndUri Domain, IPublishedContent RootNode)> domains = allDomains
            .Select(d => new DomainAndUri(d, currentUri))
            .Where(d => d.Uri.Authority == currentUri.Authority)
            .Select(d => (Domain: d, RootNode: umbracoContext.Content?.GetById(d.ContentId)))
            .Where(d => d.RootNode != null) // Check for null to ensure node is published
            .Cast<(DomainAndUri Domain, IPublishedContent RootNode)>()
            .ToList();

        // If no matching domain is in the default culture or has the default path, but the default culture is published, add the default domain manually
        if (domainCache is not null
            && !domains.Any(d => d.Domain.Uri.LocalPath == "/" || d.Domain.Culture.InvariantEquals(domainCache.DefaultCulture))
            && domains.Find(d => d.RootNode.IsPublished(domainCache.DefaultCulture)).RootNode is { } rootNode)
        {
            domains.Insert(0, (new DomainAndUri(new Domain(0, "Default", rootNode.Id, domainCache.DefaultCulture, false, -1), new Uri(currentUri, "/")), rootNode));
        }

        return domains;
    }

    /// <inheritdoc />
    public ActionResult GenerateSitemapIndex(
        IEnumerable<(DomainAndUri Domain, IPublishedContent RootNode)> domains,
        Func<string, string> sitemapCultureUrlMapper)
    {
        return _sitemapProvider.CreateSitemapIndex(
            new SitemapIndexModel(domains
                .DistinctBy(d => d.Domain.Culture)
                .Where(d => d.Domain.Culture is not null)
                .Select(d => new SitemapIndexNode(sitemapCultureUrlMapper(d.Domain.Culture!)))
                .ToList()
            )
            {
                StyleSheets = [new("/sitemap.xsl")],
            }
        );
    }

    /// <inheritdoc />
    public ActionResult GenerateSitemap(
        List<(DomainAndUri Domain, IPublishedContent RootNode)> domains,
        string? culture)
    {
        if (culture is null)
        {
            // If we could not calculate the culture, return an empty sitemap.
            return _sitemapProvider.CreateSitemap(new SitemapModel([]));
        }

        List<IPublishedContent> rootNodes = domains
            .Where(d => d.Domain.Culture.InvariantEquals(culture))
            .Select(n => n.RootNode)
            .ToList();

        // Fallback to first root node when no domains are defines
        if (rootNodes.Count == 0 && _umbracoContextAccessor.GetRequiredUmbracoContext().Content?.GetAtRoot().FirstOrDefault(n => ShouldBeDisplayedInSitemap(n, culture)) is { } firstNode)
        {
            rootNodes.Add(firstNode);
        }

        return _sitemapProvider.CreateSitemap(
            new SitemapModel(rootNodes
                .SelectMany(n => n.DescendantsOrSelf(culture))
                .Where(n => ShouldBeDisplayedInSitemap(n, culture))
                .Select(n => new SitemapNode(n.Url(culture, UrlMode.Absolute))
                {
                    LastModificationDate = n.UpdateDate.ToLocalTime().Date,
                    ChangeFrequency = GetChangeFrequency(n),
                    Priority = GetPriority(n),
                    Translations = _globalizationService.GetAlternateUrls(n) is { Count: > 1 } alternateUrls // Has other cultures
                        ? alternateUrls.ConvertAll(u => new SitemapPageTranslation(u.Url, u.Lang))
                        : [],
                })
                .ToList()
            )
            {
                StyleSheets = [new("/sitemap.xsl")],
            }
        );
    }

    public ActionResult GenerateEmptySitemap()
    {
        return _sitemapProvider.CreateSitemap(new SitemapModel([]));
    }

    private bool ShouldBeDisplayedInSitemap(IPublishedContent node, string culture)
    {
        if (node.TemplateId is null or 0)
        {
            return false;
        }

        // Temporarily switch to the correct culture so everything is regarding the correct culture
        using (new VariationContextHelper(_variationContextAccessor, culture))
        {
            if (node is ICompositionSeo { DoNotIndex: true })
            {
                return false;
            }

            if (string.IsNullOrEmpty(culture))
            {
                return true;
            }

            // Extra checks for multi cultural content
            return _globalizationService.IsPublishedAndRoutable(node, culture);
        }
    }
}
