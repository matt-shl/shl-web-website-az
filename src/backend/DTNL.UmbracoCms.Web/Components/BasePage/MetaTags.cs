using DTNL.UmbracoCms.Web.Helpers.Extensions;
using DTNL.UmbracoCms.Web.Models.BrandfolderAssets;
using DTNL.UmbracoCms.Web.Models.Globalization;
using DTNL.UmbracoCms.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components.BasePage;

public class MetaTags : ViewComponentExtended
{
    private readonly IGlobalizationService _globalizationService;

    public MetaTags(IGlobalizationService globalizationService)
    {
        _globalizationService = globalizationService;
    }

    public required string Title { get; set; }

    public required string Description { get; set; }

    public required string Robots { get; set; }

    public required string Language { get; set; }

    public string? CanonicalUrl { get; set; }

    public required List<AlternateUrl> AlternateLanguageUrls { get; set; }

    public required OpenGraphMetaTags OpenGraph { get; set; }

    public required TwitterMetaTags Twitter { get; set; }

    public string? ApplicationName { get; set; }

    public IViewComponentResult Invoke(IPublishedContent page, SiteSettings? siteSettings)
    {
        bool isErrorPage = HttpContext.Response.StatusCode is < 200 or > 299;
        string? websiteName = siteSettings?.WebsiteName;

        Title = GetTitle(page, websiteName);
        Description = GetMetaDescription(page);
        Robots = GetRobots(page, isErrorPage);
        Language = System.Globalization.CultureInfo.CurrentCulture.ToString();
        CanonicalUrl = !isErrorPage ? page.Url(mode: UrlMode.Absolute) : null;
        AlternateLanguageUrls = !isErrorPage ? _globalizationService.GetAlternateUrls(page) : [];

        OpenGraph = GetOpenGraph(page);
        Twitter = GetTwitter(page);

        ApplicationName = siteSettings?.WebsiteName;

        return View("~/Components/BasePage/MetaTags.cshtml", this);
    }

    private static string GetTitle(IPublishedContent page, string? websiteNameSuffix = null)
    {
        string title = (page as ICompositionSeo)?.SeoMetaTitle.NullOrEmptyAsNull() ?? page.GetTitle();

        if (!string.IsNullOrEmpty(websiteNameSuffix))
        {
            title = title.EnsureEndsWith($" | {websiteNameSuffix}");
        }

        return title;
    }

    private static string GetMetaDescription(IPublishedContent page)
    {
        string? metaDescription = (page as ICompositionSeo)?.SeoMetaDescription;
        if (!metaDescription.IsNullOrEmpty())
        {
            return metaDescription;
        }

        string? pageIntro = (page as ICompositionBasePage)?.GetDescription().RemoveHtml().TruncateOnWholeWord(156);
        return pageIntro ?? "";
    }

    private static string GetRobots(IPublishedContent page, bool isErrorPage)
    {
        if (isErrorPage)
        {
            return "noindex,nofollow";
        }

        if (page is not ICompositionSeo compositionSeo)
        {
            return "index,follow";
        }

        string metaFollowContent = compositionSeo.DoNotFollow ? "nofollow" : "follow";
        string metaIndexContent = compositionSeo.DoNotIndex ? "noindex" : "index";
        return $"{metaIndexContent},{metaFollowContent}";
    }

    private static OpenGraphMetaTags GetOpenGraph(IPublishedContent page)
    {
        ICompositionSocialSharing? socialSharing = page as ICompositionSocialSharing;

        string ogTitle = socialSharing?.OgTitle.NullOrEmptyAsNull() ?? GetTitle(page);
        string ogDescription = socialSharing?.OgDescription.NullOrEmptyAsNull() ?? GetMetaDescription(page);

        string? ogImageUrl = BrandfolderAttachment
            .Create(socialSharing?.OgImage ?? GetHeroImage(page))?
            .GetDefaultCropUrl(1200, 630);

        return new OpenGraphMetaTags
        {
            Title = ogTitle,
            Description = ogDescription,
            ImageUrl = ogImageUrl,
        };
    }

    private static TwitterMetaTags GetTwitter(IPublishedContent page)
    {
        ICompositionSocialSharing? socialSharing = page as ICompositionSocialSharing;

        string twitterTitle = socialSharing?.TwitterTitle.NullOrEmptyAsNull() ?? socialSharing?.OgTitle.NullOrEmptyAsNull() ?? GetTitle(page);
        string twitterDescription = socialSharing?.TwitterDescription.NullOrEmptyAsNull() ?? socialSharing?.OgDescription.NullOrEmptyAsNull() ?? GetMetaDescription(page);

        string? twitterImageUrl = BrandfolderAttachment
            .Create(socialSharing?.TwitterImage ?? socialSharing?.OgImage ?? GetHeroImage(page))?
            .GetDefaultCropUrl(1200, 630);

        return new TwitterMetaTags
        {
            Title = twitterTitle,
            Description = twitterDescription,
            ImageUrl = twitterImageUrl,
        };
    }

    private static string? GetHeroImage(IPublishedContent page)
    {
        if (page is not ICompositionHero heroPage)
        {
            return null;
        }

        return heroPage.Hero?.FirstOrDefault()?.Content switch
        {
            NestedBlockProductHero productHero => productHero.Image,
            _ => null,
        };
    }
}
