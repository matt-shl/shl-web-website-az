using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class CardImage
{
    public required string Title { get; set; }

    public string? Text { get; set; }

    public Image? Image { get; set; }

    public string? Url { get; set; }

    public bool HasUrl { get; set; }

    public string? ThemeCssClass { get; set; }

    public string Element => HasUrl ? "a" : "article";

    public CardImage? Create(NestedBlockImageCard imageCard, string? themeCssClass)
    {
        if (imageCard.Title.IsNullOrWhiteSpace())
        {
            return null;
        }

        return new CardImage
        {
            Title = imageCard.Title,
            Text = imageCard.Text?.ToHtmlString(),
            Image = Image.Create(imageCard.Image),
            Url = imageCard.Link?.Url,
            HasUrl = imageCard.Link is not null,
            ThemeCssClass = themeCssClass,
        };
    }
}
