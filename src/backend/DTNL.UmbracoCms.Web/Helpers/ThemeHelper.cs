using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Helpers;

public static class ThemeHelper
{
    public static string GetCssClasses(IPublishedContent? page)
    {
        if (page?.ContentType.Alias is "pageHome")
        {
            string theme = (page as ICompositionBasePage)?.ThemeHomePage?.Label ?? "general";

            return $"t-{theme}";
        }

        if (page?.ContentType.Alias is "pageContent")
        {
            string theme = (page as ICompositionBasePage)?.ThemeContentPage?.Label ?? "general";

            return $"t-{theme}";
        }

        return $"t-general";
    }
}
