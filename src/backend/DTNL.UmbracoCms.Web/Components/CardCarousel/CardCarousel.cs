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

    public bool ShowThreeSideBySide { get; set; }

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

        NestedBlockButtonLink? primaryLinkButtonContent = cardsBlock.PrimaryLink?.FirstOrDefault()?.Content as NestedBlockButtonLink;
        NestedBlockButtonLink? secondaryLinkButtonContent = cardsBlock.SecondaryLink?.FirstOrDefault()?.Content as NestedBlockButtonLink;


        return new CardCarousel
        {
            AnchorId = (cardsBlock as ICompositionAnchors)?.AnchorId,
            AnchorTitle = (cardsBlock as ICompositionAnchors)?.AnchorTitle,
            Title = cardsBlock.Title,
            Text = cardsBlock.Text?.ToHtmlString(),
            PrimaryLinkButton = Button.Create(primaryLinkButtonContent)
                .With(b =>
                {
                    b.Variant = b.Variant ?? "primary";
                    b.Icon = b.Icon ?? SvgAliases.Icons.ArrowTopRight;
                }),
            SecondaryLinkButton = Button
                .Create(secondaryLinkButtonContent)
                .With(b =>
                {
                    b.Variant = b.Variant ?? "primary";
                    b.Icon = b.Icon ?? SvgAliases.Icons.ArrowTopRight;
                }),
            Cards = cards,
            ShowCarousel = cards.Count > 3 || (cardsBlock.ShowCarousel && cards.Count == 3),
            ShowThreeSideBySide = !cardsBlock.ShowCarousel && cards.Count == 3,
        };
    }
}
