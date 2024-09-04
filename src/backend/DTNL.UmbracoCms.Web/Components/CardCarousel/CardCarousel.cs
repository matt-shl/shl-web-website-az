using DTNL.UmbracoCms.Web.Helpers.Aliases;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
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

    public static CardCarousel? Create(ICompositionCards cardsBlock)
    {
        List<ICard> cards = cardsBlock switch
        {
            NestedBlockContactCards block => block.Cards
                .Using(cardBlock => ICard.Create(cardBlock.Content, cssClasses: "section-card-carousel__card"))
                .ToList(),
            NestedBlockGenericCards block => block.Cards
                .Using(cardBlock => ICard.Create(cardBlock.Content, cssClasses: "section-card-carousel__card"))
                .ToList(),
            NestedBlockKnowledgeCards block => block.Cards
                .Using(cardBlock => ICard.Create(cardBlock.Content, cssClasses: "section-card-carousel__card"))
                .ToList(),
            NestedBlockProductCards block => block.Cards
                .Using(cardBlock => ICard.Create(cardBlock.Content, cssClasses: "section-card-carousel__card"))
                .ToList(),
            _ => [],
        };

        if (cards.Count == 0)
        {
            return null;
        }

        return new CardCarousel
        {
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
