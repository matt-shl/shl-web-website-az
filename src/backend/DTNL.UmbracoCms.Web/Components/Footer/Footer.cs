using DTNL.UmbracoCms.Web.Components.BasePage;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class Footer : ViewComponentExtended
{
    public required List<Link> Links { get; set; }

    public SocialLinks? SocialLinks { get; set; }

    public IViewComponentResult Invoke(SiteSettings? siteSettings)
    {
        Links = siteSettings?.FooterLinks?.Select(l => new Link { Url = l.Url ?? "", Label = l.Name }).ToList() ?? [];
        SocialLinks = SocialLinks.Create(siteSettings);

        return View("Footer", this);
    }
}
