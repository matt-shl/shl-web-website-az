using Umbraco.Cms.Core.Models.PublishedContent;

namespace DTNL.UmbracoCms.Web.Components.NestedBlock;

public class NestedBlockQuotesBanner : NestedBlockWithInner
{
    protected override BannerQuote? GetInnerComponent(IPublishedElement block)
    {
        if (block is not Umbraco.Cms.Web.Common.PublishedModels.NestedBlockQuotesBanner quoteBannerBlock)
        {
            return null;
        }

        LayoutSection.Variant = LayoutSection.CssThemeClasses is not null ? "in-grid" : null;

        return BannerQuote.Create(quoteBannerBlock);
    }
}
