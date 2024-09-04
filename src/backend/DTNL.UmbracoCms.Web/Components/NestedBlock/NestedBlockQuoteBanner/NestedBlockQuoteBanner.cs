using Umbraco.Cms.Core.Models.PublishedContent;

namespace DTNL.UmbracoCms.Web.Components.NestedBlock;

public class NestedBlockQuoteBanner : NestedBlockWithInner
{
    protected override BannerQuote? GetInnerComponent(IPublishedElement block)
    {
        if (block is not Umbraco.Cms.Web.Common.PublishedModels.NestedBlockQuoteBanner quoteBannerBlock)
        {
            return null;
        }

        if (BannerQuote.Create(quoteBannerBlock) is not { } quoteBanner)
        {
            return null;
        }

        LayoutSection.CssClasses = quoteBannerBlock.Theme != null ? $"t-{quoteBannerBlock?.Theme?.Label}" : "t-white";
        LayoutSection.Variant = quoteBannerBlock?.Theme != null ? "in-grid" : "";
        LayoutSection.Id = quoteBanner.AnchorId;
        LayoutSection.NavigationTitle = quoteBanner.AnchorTitle;

        return quoteBanner;
    }
}
