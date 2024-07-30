using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class CardIcon
{
    public required string Title { get; set; }

    public string? Text { get; set; }

    public string? IconUrl { get; set; }

    public string? IconAlias { get; set; }

    public string? Url { get; set; }

    public bool HasUrl { get; set; }

    public string? ThemeCssClass { get; set; }

    public string Element => HasUrl ? "a" : "article";

    public CardIcon? Create(NestedBlockIconCard iconCard, string? themeCssClass)
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
            ThemeCssClass = themeCssClass,
        };
    }
}
