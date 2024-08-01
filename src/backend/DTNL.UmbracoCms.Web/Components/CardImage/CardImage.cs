using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class CardImage : ICard
{
    public required string Title { get; set; }

    public string? SubTitle { get; set; }

    public string? Text { get; set; }

    public Image? Image { get; set; }

    public string? Url { get; set; }

    public bool HasUrl { get; set; }

    public string? CssClasses { get; set; }

    public string Element => HasUrl ? "a" : "article";

    public static CardImage? Create(NestedBlockImageCard imageCard, string? cssClasses = null)
    {
        if (imageCard.Title.IsNullOrWhiteSpace())
        {
            return null;
        }

        return new CardImage
        {
            Title = imageCard.Title,
            SubTitle = imageCard.SubTitle,
            Text = imageCard.Text?.ToHtmlString(),
            Image = Image.Create(imageCard.Image, cssClasses: "card-image__image"),
            Url = imageCard.Link?.Url,
            HasUrl = imageCard.Link is not null,
            CssClasses = cssClasses,
        };
    }
}
