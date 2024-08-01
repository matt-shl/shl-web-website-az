using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Umbraco.Cms.Core.Constants.HttpContext;

namespace DTNL.UmbracoCms.Web.Components;

public class Header : ViewComponentExtended
{
    public NestedBlockNavigation? Navigation { get; set; }

    public IViewComponentResult Invoke()
    {
        SiteSettings? siteSettings = NodeProvider.SiteSettings;
        NestedBlockNavigation? navigation = GetNavigation(siteSettings);

        if (navigation is not null)
        {
            Navigation = navigation;
        }

        return View("Header", this);
    }

    private NestedBlockNavigation? GetNavigation(SiteSettings? settings)
    {
       return settings!.MainHeader!.Content;
    }
}
