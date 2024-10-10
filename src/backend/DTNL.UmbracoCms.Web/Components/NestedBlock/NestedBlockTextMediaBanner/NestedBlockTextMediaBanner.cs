using Umbraco.Cms.Core.Models.PublishedContent;

namespace DTNL.UmbracoCms.Web.Components.NestedBlock;

public class NestedBlockTextMediaBanner : NestedBlockWithInner
{
    protected override BannerTextMedia? GetInnerComponent(IPublishedElement block)
    {
        if (block is not Umbraco.Cms.Web.Common.PublishedModels.NestedBlockTextMediaBanner textMediaBannerBlock)
        {
            return null;
        }

        if (LayoutSection.CssThemeClasses is not null)
        {
            LayoutSection.Variant = LayoutSection.CssThemeClasses is "t-white" ? "in-grid" : null;
        }

        return BannerTextMedia.Create(textMediaBannerBlock);
    }
}
