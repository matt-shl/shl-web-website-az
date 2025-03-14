using System.Diagnostics.CodeAnalysis;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Helpers;

public static class ThemeHelper
{
    public static string GetCssClasses(IPublishedContent? page)
    {
        if (page is PageVacancy vacancyPage)
        {
            return GetCssClasses(vacancyPage.PageTheme ?? (vacancyPage.Parent as PageVacancyOverview)?.VacanciesPageTheme, "light-blue");
        }

        return GetCssClasses((page as IPageTheme)?.PageTheme, "light-blue");
    }

    [return: NotNullIfNotNull(nameof(fallBackTheme))]
    public static string? GetCssClasses(ColorPickerValueConverter.PickedColor? theme, string? fallBackTheme = null)
    {
        return (theme?.Label ?? fallBackTheme)?.EnsureStartsWith("t-");
    }
}
