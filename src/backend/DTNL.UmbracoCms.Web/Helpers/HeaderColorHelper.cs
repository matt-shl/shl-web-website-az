using Umbraco.Cms.Core.Models.PublishedContent;

namespace DTNL.UmbracoCms.Web.Helpers;

public static class HeaderColorHelper
{
    public static string? GetColor(IPublishedContent? page)
    {
        return page?.ContentType.Alias is "pageHome" or "pageProductDetail" ? "is--header-white" : null;
    }
}
