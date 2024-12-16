using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Helpers;

public static class HeaderColorHelper
{
    public static string? GetColor(IPublishedContent? page)
    {
        return (page as IPageHero)?.Hero?.FirstOrDefault()?.Content switch
        {
            NestedBlockHomepageHero or NestedBlockProductHero => "is--header-white",
            _ => null,
        };
    }
}
