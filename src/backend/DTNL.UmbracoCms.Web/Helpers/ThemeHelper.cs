using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Helpers;

public static class ThemeHelper
{
    public static string GetCssClasses(IPublishedContent? page)
    {
        return GetCssClasses((page as IPageTheme)?.PageTheme, "light-blue");
    }

    public static string GetCssClasses(ColorPickerValueConverter.PickedColor? theme, string fallBackTheme)
    {
        return (theme?.Label ?? fallBackTheme).EnsureStartsWith("t-");
    }
}
