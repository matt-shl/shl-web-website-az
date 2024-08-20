using Umbraco.Cms.Core.Models.PublishedContent;

namespace DTNL.UmbracoCms.Web.Components.NestedBlock.TextMediaBanner;

public class NestedBlockTextMediaBanner : NestedBlockWithInner
{
    protected override BannerTextMedia? GetInnerComponent(IPublishedElement block)
    {
        if (block is not Umbraco.Cms.Web.Common.PublishedModels.NestedBlockTextMediaBanner textMediaBannerBlock)
        {
            return null;
        }

        if (BannerTextMedia.Create(textMediaBannerBlock) is not { } textMediaBanner)
        {
            return null;
        }

        LayoutSection.CssClasses = textMediaBannerBlock.Theme != null ? $"t-{textMediaBannerBlock?.Theme?.Label}" : "t-white";
        LayoutSection.Variant = "in-grid";
        Id = textMediaBanner.Id;

        return textMediaBanner;
    }
}
