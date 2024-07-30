using DTNL.UmbracoCms.Web.Helpers.Aliases;
using Flurl;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components.BasePage;

public class SocialLinks
{
    private static readonly IReadOnlyList<(string Domain, string Icon)> SocialLinkPlatforms =
        [
            ("facebook.com", SvgAliases.Icons.SocialFacebook),
            ("x.com", SvgAliases.Icons.SocialX),
            ("linkedin.com", SvgAliases.Icons.SocialLinkedin),
            ("instagram.com", SvgAliases.Icons.SocialInstagram),
            ("youtube.com", SvgAliases.Social.Youtube),
        ];

    public required List<Link> Links { get; set; }

    public static SocialLinks? Create(SiteSettings? settings)
    {
        if (settings is not ICompositionSocialLinks socialLinks || socialLinks.SocialLinks?.Any() != true)
        {
            return null;
        }

        List<Link> links = socialLinks
            .SocialLinks
            .Select(GetSocialLink)
            .WhereNotNull()
            .ToList();

        return new SocialLinks { Links = links };
    }

    private static Link? GetSocialLink(Umbraco.Cms.Core.Models.Link? link)
    {
        if (link?.Url is null or "")
        {
            return null;
        }

        Url url = new(link.Url);
        if (url.IsRelative)
        {
            return null;
        }

        (string Domain, string Icon) socialPlatform = SocialLinkPlatforms.FirstOrDefault(s => url.Host.EndsWith(s.Domain, StringComparison.OrdinalIgnoreCase));

        return socialPlatform == default
            ? null
            : Link.Create(link, icon: socialPlatform.Icon, hideLabel: true);
    }
}
