using Umbraco.Cms.Core.Models.PublishedContent;

namespace DTNL.UmbracoCms.Web.Components.NestedBlock.NestedBlockStackedCards;

public class NestedBlockStackingCards : NestedBlockWithInner
{
    protected override StackingCards? GetInnerComponent(IPublishedElement block)
    {
        if (block is not Umbraco.Cms.Web.Common.PublishedModels.NestedBlockStackingCards stackingCardsBlock)
        {
            return null;
        }

        LayoutSection.CssThemeClasses = "t-white";
        LayoutSection.CssClasses = "u-overflow-visible";

        return StackingCards.Create(stackingCardsBlock);
    }
}
