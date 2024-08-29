using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Helpers;

public static class ThemeHelper
{
    public static string GetCssClasses(IPublishedContent? page)
    {
        if (page is ICompositionBasePage basePage)
        {
            string theme = basePage.PageTheme?.Label ?? "general";

            return $"t-{theme}";
        }

        return "t-general";

    }
}
