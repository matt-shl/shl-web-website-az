using Umbraco.Cms.Core.Models.PublishedContent;

namespace DTNL.UmbracoCms.Web.Components.NestedBlock;

public class NestedBlockKnowledgeCards : NestedBlockWithInner
{
    protected override CardCarousel? GetInnerComponent(IPublishedElement block)
    {
        if (block is not Umbraco.Cms.Web.Common.PublishedModels.NestedBlockKnowledgeCards cardsBlock)
        {
            return null;
        }

        if (CardCarousel.Create(cardsBlock) is not { } cardCarousel)
        {
            return null;
        }

        SetCarouselsLayout(cardCarousel);

        return cardCarousel;
    }
}
