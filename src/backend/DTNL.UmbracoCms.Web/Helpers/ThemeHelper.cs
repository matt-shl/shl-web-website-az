using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Helpers;

public static class ThemeHelper
{
    public static string GetCssClasses(IPublishedContent page)
    {
        if (page.ContentType.Alias is "pageHome")
        {
            string theme = (page as ICompositionHomePage)?.Theme?.Label ?? "general";

            return $"t-{theme}";
        }

        if (page is ICompositionBasePage)
        {
            string theme = (page as ICompositionBasePage)?.Theme?.Label ?? "general";

            return $"t-{theme}";
        }

        return $"t-general";
    }
}
