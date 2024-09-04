using DTNL.UmbracoCms.Web.Helpers.Aliases;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class ImageCarousel : CardCarousel
{
    public static ImageCarousel? Create(NestedBlockImageCards cardsBlock)
    {
        List<ICard> cards = cardsBlock.Cards
            .Using(cardBlock => ICard.Create(cardBlock.Content, cssClasses: "section-image-carousel__image"))
            .ToList();

        if (cards.Count == 0)
        {
            return null;
        }

        return new ImageCarousel
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
