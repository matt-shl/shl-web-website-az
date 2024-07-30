using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Helpers;

public static class ThemeHelper
{
    public static string GetCssClasses(IPublishedContent page)
    {
        string theme = (page as ICompositionBasePage)?.Theme?.Label ?? "general";

        return $"t-{theme}";
    }
}
