using System.Globalization;
using DTNL.UmbracoCms.Web.Helpers;
using DTNL.UmbracoCms.Web.Infrastructure.Configuration.Options;
using DTNL.UmbracoCms.Web.Infrastructure.DependencyInjection;
using DTNL.UmbracoCms.Web.Models.Globalization;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;

namespace DTNL.UmbracoCms.Web.Services;

public interface IGlobalizationService
{
    /// <summary>
    /// Retrieves the alternate languages for a specific <paramref name="node"/>.
    /// </summary>
    /// <remarks>The default language for the node will be either the default language of Umbraco, if it exists, or the first language retrieved.</remarks>
    public List<AlternateUrl> GetAlternateUrls(IPublishedContent node, bool filterNonCrawlable = true);

    /// <summary>
    /// Checks if the provided content <param name="node"></param> is published and is routable for the provided culture.
    /// </summary>
    bool IsPublishedAndRoutable(IPublishedContent node, string culture);
}

[Transient]
public class GlobalizationService : IGlobalizationService
{
    private readonly IVariationContextAccessor _variationContextAccessor;
    private readonly IUmbracoContextAccessor _umbracoContextAccessor;
    private readonly IOptionsMonitor<ApplicationOptions> _applicationOptions;

    public GlobalizationService(IVariationContextAccessor variationContextAccessor, IUmbracoContextAccessor umbracoContextAccessor, IOptionsMonitor<ApplicationOptions> applicationOptions)
    {
        _variationContextAccessor = variationContextAccessor;
        _umbracoContextAccessor = umbracoContextAccessor;
        _applicationOptions = applicationOptions;
    }

    public List<AlternateUrl> GetAlternateUrls(IPublishedContent node, bool filterNonCrawlable = true)
    {
        string? defaultCulture = _umbracoContextAccessor.GetRequiredUmbracoContext().Domains?.DefaultCulture;

        List<AlternateUrl> alternateUrls = node.Cultures
            .Where(c => IsPublishedAndRoutable(node, c.Key))
            .Select(cultureAndInfo => new AlternateUrl
            {
                LanguageName = new CultureInfo(cultureAndInfo.Value.Culture).NativeName,
                LanguageCode = cultureAndInfo.Key,
                Url = node.Url(cultureAndInfo.Key, UrlMode.Absolute),
                IsDefault = defaultCulture is not null && cultureAndInfo.Key.Equals(defaultCulture, StringComparison.OrdinalIgnoreCase),
            })
            .Where(u => !filterNonCrawlable || _applicationOptions.CurrentValue.IsCrawlableUrl(new Uri(u.Url)))
            .ToList();

        // If there is no node in the default language, just set the first one as default.
        if (alternateUrls.Count > 0 && !alternateUrls.Any(u => u.IsDefault))
        {
            alternateUrls[0].IsDefault = true;
        }

        return alternateUrls;
    }

    public bool IsPublishedAndRoutable(IPublishedContent node, string culture)
    {
        // Check current culture is published
        if (!node.IsPublished(culture))
        {
            return false;
        }

        // When a parent node is not published for the current culture the url is '#'
        if (node.Url(culture, UrlMode.Relative) is "#")
        {
            return false;
        }

        // HACK: Setting the variation context due to issue https://github.com/umbraco/Umbraco-CMS/issues/10858
        // When a node has not been created in the main language, the other languages return null in GetCultureFromDomains
        using (new VariationContextHelper(_variationContextAccessor, culture))
        {
            // Check for conflicts causing the node url for a specific culture to resolve to a different one
            if (!culture.InvariantEquals(node.GetCultureFromDomains(new Uri(node.Url(culture, UrlMode.Absolute)))))
            {
                return false;
            }
        }

        return true;
    }
}
