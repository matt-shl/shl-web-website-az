using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class CardIcon : ICard
{
    public required string Title { get; set; }

    public string? Text { get; set; }

    public Image? Icon { get; set; }

    public string? IconURL { get; set; }

    public string? IconAlias { get; set; }

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
            //IconUrl = iconCard.SvgIcon?.MediaUrl(),
            //IconAlias = iconCard.Icon,
            Icon = Image.Create(iconCard.Icon),
            Url = iconCard.Link?.Url,
            CssClasses = cssClasses,
        };
    }
}
