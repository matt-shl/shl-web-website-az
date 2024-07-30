using Umbraco.Cms.Core.Models.PublishedContent;

namespace DTNL.UmbracoCms.Web.Components.NestedBlock;

public class NestedBlockCardCarousel : NestedBlockWithInner
{
    protected override CardCarousel? GetInnerComponent(IPublishedElement block)
    {
        //if (block is not Umbraco.Cms.Web.Common.PublishedModels.BigQuoteIllustrationBlock quoteIllustrationBlock)
        //{
        //    return null;
        //}

        // TODO
        if (CardCarousel.Create(null!) is not { } cardCarousel)
        {
            return null;
        }

        LayoutSection.CssClasses = cardCarousel.ShowCarousel
            ? "c-section-card-carousel c-section-card-carousel--no-carousel-three"
            : "c-section-card-carousel--show-carousel";

        return cardCarousel;
    }
}
