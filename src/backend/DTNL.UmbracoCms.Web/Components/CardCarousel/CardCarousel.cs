using DTNL.UmbracoCms.Web.Helpers.Aliases;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class CardCarousel
{
    public string? AnchorId { get; set; }

    public string? AnchorTitle { get; set; }
    public string? Title { get; set; }

    public string? Text { get; set; }

    public Button? PrimaryLinkButton { get; set; }

    public Button? SecondaryLinkButton { get; set; }

    public required List<ICard> Cards { get; set; }

    public bool ShowCarousel { get; set; }

    public static CardCarousel? Create(NestedBlockCards cardsBlock)
    {
        List<ICard> cards = cardsBlock.Cards
            .Using(cardBlock => ICard.Create(cardBlock.Content, cssClasses: "section-card-carousel__card"))
            .ToList();

        if (cards.Count == 0)
        {
            return null;
        }

        return new CardCarousel
        {
            AnchorId = cardsBlock.AnchorId,
            AnchorTitle = cardsBlock.AnchorTitle,
            Title = cardsBlock.Title,
            Text = cardsBlock.Text?.ToHtmlString(),
            PrimaryLinkButton = Button
                .Create(cardsBlock.PrimaryLink)
                .With(b => b.Icon = SvgAliases.Icons.ArrowTopRight),
            SecondaryLinkButton = Button
                .Create(cardsBlock.SecondaryLink)
                .With(b =>
                {
                    b.Icon = SvgAliases.Icons.ArrowTopRight;
                    b.Variant = "secondary";
                }),
            Cards = cards,
            ShowCarousel = cards.Count > 3 || (cardsBlock.ShowCarousel && cards.Count == 3),
        };
    }
}
