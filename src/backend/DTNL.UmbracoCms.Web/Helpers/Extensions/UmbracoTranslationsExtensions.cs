using System.Globalization;
using Umbraco.Cms.Core.Dictionary;
using Umbraco.Cms.Web.Common;

namespace DTNL.UmbracoCms.Web.Helpers.Extensions;

public static class UmbracoTranslationsExtensions
{
    /// <summary>
    /// Retrieves the translation corresponding to the given <paramref name="key"/> from Umbraco.
    /// </summary>
    /// <remarks>If not found, the given <paramref name="key"/> is returned instead.</remarks>
    public static string GetTranslation(this UmbracoHelper umbHelper, string key)
    {
        return umbHelper.GetDictionaryValueOrDefault(key, "{{" + key + "}}");
    }

    /// <summary>
    /// Retrieves the translation corresponding to the given <paramref name="key"/> from Umbraco.
    /// </summary>
    /// <remarks>If not found, the given <paramref name="key"/> is returned instead.</remarks>
    public static string GetTranslation(this ICultureDictionary cultureDictionary, string key)
    {
        return cultureDictionary.GetTranslationOrDefault(key, defaultValue: "{{" + key + "}}");
    }

    /// <summary>
    /// Retrieves the translation corresponding to the given <paramref name="key"/> from Umbraco or the specified <paramref name="defaultValue"/>.
    /// </summary>
    public static string GetTranslationOrDefault(this ICultureDictionary cultureDictionary, string key, string defaultValue)
    {
        string? dictionaryValue = cultureDictionary[key];
        return !string.IsNullOrWhiteSpace(dictionaryValue) ? dictionaryValue : defaultValue;
    }

    /// <summary>
    /// Retrieves the translation corresponding to the given <paramref name="key"/> from Umbraco, and formats it using the specified <paramref name="parameters"/>.
    /// </summary>
    /// <remarks>If not found, the given <paramref name="key"/> is returned instead.</remarks>
    public static string GetTranslation(this ICultureDictionary cultureDictionary, string key, params object[] parameters)
    {
        return string.Format(CultureInfo.InvariantCulture, cultureDictionary.GetTranslation(key), parameters);
    }
}
