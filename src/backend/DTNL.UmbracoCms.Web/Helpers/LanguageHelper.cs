using System.Globalization;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace DTNL.UmbracoCms.Web.Helpers;

public static class LanguageHelper
{
    public static string GetLanguageName(CultureInfo cultureInfo)
    {
        return GetLanguageName(cultureInfo.TwoLetterISOLanguageName);
    }

    public static string GetLanguageName(PublishedCultureInfo cultureInfo)
    {
        return GetLanguageName(cultureInfo.Culture);
    }

    public static string GetLanguageName(string cultureCode)
    {
        return new CultureInfo(cultureCode.Split("-").First()).NativeName;
    }
}
