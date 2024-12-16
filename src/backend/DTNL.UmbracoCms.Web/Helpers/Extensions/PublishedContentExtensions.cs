using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using DTNL.UmbracoCms.Web.Components.Hero;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

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
        string? title = null;

        if (content is ICompositionHero compositionHero &&
            compositionHero.Hero?.FirstOrDefault()?.Content is { } hero)
        {
            title = hero.Value<string>(nameof(IHero.Title).ToFirstLowerInvariant());
        }

        return title.FallBack(content.Name);
    }

    /// <summary>
    /// Returns a description for the page.
    /// </summary>
    public static string GetDescription(this ICompositionBasePage content)
    {
        string? cardDescription = null;

        if (content is ICompositionHero compositionHero &&
            compositionHero.Hero?.FirstOrDefault()?.Content is { } hero)
        {
            cardDescription = hero switch
            {
                NestedBlockProductHero productHero => productHero.Text?.ToHtmlString(),
                _ => null,
            };
        }

        return cardDescription ?? "";
    }

    /// <summary>
    /// Returns a card image for the page.
    /// </summary>
    public static string? GetCardImage(this ICompositionBasePage content)
    {
        string? cardImage = (content as ICompositionCardDetails)?.CardImage;

        if (cardImage is null &&
            content is ICompositionHero compositionHero &&
            compositionHero.Hero?.FirstOrDefault()?.Content is { } hero)
        {
            cardImage = hero switch
            {
                NestedBlockProductHero productHero => productHero.Image,
                _ => null,
            };
        }

        return cardImage;
    }

    /// <summary>
    /// Returns a card description for the page.
    /// </summary>
    public static string GetCardDescription(this ICompositionBasePage content)
    {
        string? cardDescription = (content as ICompositionCardDetails)?.CardDescription?.ToHtmlString();

        return cardDescription.FallBack(content.GetDescription());
    }

    /// <summary>
    /// Returns a description for the page.
    /// </summary>
    public static string? GetCategory(this ICompositionBasePage content)
    {
        string? category = "";

        if (content is ICompositionCardDetails cardDetails)
        {
            category = string.Join(',', cardDetails.CardCategory.OrEmptyIfNull());
        }

        if (content is ICompositionContentDetails contentDetails)
        {
            category.FallBack(string.Join(',', contentDetails.ContentTags.OrEmptyIfNull()));
        }

        return category.FallBack(null);
    }

    /// <summary>
    /// Checks whether a provided <paramref name="node"/> is a folder.
    /// </summary>
    public static bool IsFolder(this IPublishedContent node)
    {
        return node.ContentType.Alias.EndsWith("folder", StringComparison.OrdinalIgnoreCase);
    }
}
