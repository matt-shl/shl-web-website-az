using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class CardCarousel
{
    public string? Title { get; set; }

    public string? Text { get; set; }

    public Button? PrimaryLinkButton { get; set; }

    public Button? SecondaryLinkButton { get; set; }

    public required List<ICard> Cards { get; set; }

    public bool ShowCarousel { get; set; }

    // TODO
    public static CardCarousel? Create(ICompositionBasePage basePage)
    {
        return new CardCarousel
        {
            Title = null,
            Cards = [],
        };
    }
}
