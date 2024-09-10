using Umbraco.Cms.Core.Models.PublishedContent;

namespace DTNL.UmbracoCms.Web.Components.NestedBlock;

public class NestedBlockImageCards : NestedBlockWithInner
{
    protected override ImageCarousel? GetInnerComponent(IPublishedElement block)
    {
        if (block is not Umbraco.Cms.Web.Common.PublishedModels.NestedBlockImageCards cardsBlock)
        {
            return null;
        }

        if (ImageCarousel.Create(cardsBlock) is not { } cardCarousel)
        {
            return null;
        }

        SetCarouselsLayout(cardCarousel, cardsBlock.Theme?.Label);

        LayoutSection.CssClasses += " c-section-image-carousel";

        return cardCarousel;
    }
}
