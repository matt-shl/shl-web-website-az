using Umbraco.Cms.Core.Models.PublishedContent;

namespace DTNL.UmbracoCms.Web.Components.NestedBlock;

public class NestedBlockCards : NestedBlockWithInner
{
    protected override CardCarousel? GetInnerComponent(IPublishedElement block)
    {
        if (block is not Umbraco.Cms.Web.Common.PublishedModels.NestedBlockCards cardsBlock)
        {
            return null;
        }

        if (CardCarousel.Create(cardsBlock) is not { } cardCarousel)
        {
            return null;
        }

        LayoutSection.CssClasses = cardCarousel.ShowCarousel
            ? "c-section-card-carousel c-section-card-carousel--show-carousel"
            : "c-section-card-carousel c-section-card-carousel--no-carousel-three";
        LayoutSection.Id = cardCarousel.AnchorId;
        LayoutSection.NavigationTitle = cardCarousel.AnchorTitle;

        return cardCarousel;
    }
}
