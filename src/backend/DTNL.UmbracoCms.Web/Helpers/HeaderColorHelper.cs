using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Helpers;

public static class HeaderColorHelper
{
    public static string GetColor(IPublishedContent? page)
    {
        string colorClass = (page as ICompositionBasePage)?.ContentType.Alias is "pageHome" or "pageProductDetail" ? "is--header-white" : "";

        return colorClass;
    }
}
