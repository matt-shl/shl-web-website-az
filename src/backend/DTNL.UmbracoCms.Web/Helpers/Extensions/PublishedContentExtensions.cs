using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace DTNL.UmbracoCms.Web.Helpers.Extensions;

public static class PublishedContentExtensions
{
    /// <summary>
    /// Returns the value of the <paramref name="property"/>. If the value is default, the <paramref name="defaultValue"/> will be returned instead.
    /// </summary>
    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static TValue? ValueOrDefault<TModel, TValue>(
        this TModel content,
        Expression<Func<TModel, TValue>> property,
        TValue defaultValue
    )
        where TModel : IPublishedElement
    {
        return content.ValueFor(
            property,
            fallback: Fallback.ToDefaultValue,
            defaultValue: defaultValue);
    }

    /// <summary>
    /// Returns the title of the page. If the title is not set, the page name will be returned instead.
    /// </summary>
    public static string GetTitle(this IPublishedContent content)
    {
        if (content is Umbraco.Cms.Web.Common.PublishedModels.ICompositionBasePage { Title: { } title } && !string.IsNullOrEmpty(title))
        {
            return title;
        }

        return content.Name ?? "";
    }

    /// <summary>
    /// Checks whether a provided <paramref name="node"/> is a folder.
    /// </summary>
    public static bool IsFolder(this IPublishedContent node)
    {
        return node.ContentType.Alias.EndsWith("folder", StringComparison.OrdinalIgnoreCase);
    }
}
