using DTNL.UmbracoCms.Web.Models.BrandfolderAssets;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class CardIcon : ICard
{
    public required string Title { get; set; }

    public string? Text { get; set; }

    public string? IconSrc { get; set; }

    public string? Url { get; set; }

    public string? CssClasses { get; set; }

    public bool HasUrl => !Url.IsNullOrWhiteSpace();

    public string Element => HasUrl ? "a" : "article";

    public static CardIcon? Create(NestedBlockIconCard iconCard, string? cssClasses = null)
    {
        if (iconCard.Title.IsNullOrWhiteSpace())
        {
            return null;
        }

        return new CardIcon
        {
            Title = iconCard.Title,
            Text = iconCard.Text?.ToHtmlString(),
            Url = iconCard.Link?.Url,
            CssClasses = cssClasses,
            IconSrc = BrandfolderAttachment.GetAssetUrl(iconCard.Icon),
        };
    }
}
