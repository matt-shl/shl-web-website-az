using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class CardIcon : ICard
{
    public required string Title { get; set; }

    public string? Text { get; set; }

    public string? IconUrl { get; set; }

    public string? IconAlias { get; set; }

    public string? Url { get; set; }

    public bool HasUrl { get; set; }

    public string? CssClasses { get; set; }

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
            IconUrl = iconCard.SvgIcon?.MediaUrl(),
            IconAlias = iconCard.Icon,
            Url = iconCard.Link?.Url,
            HasUrl = iconCard.Link is not null,
            CssClasses = cssClasses,
        };
    }
}
