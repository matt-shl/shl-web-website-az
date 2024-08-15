using Umbraco.Cms.Core.Models.PublishedContent;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DTNL.UmbracoCms.Web.Components.NestedBlock.TextMediaBanner;

public class TextMediaBanner : NestedBlockWithInner
{
    protected override BannerTextMedia? GetInnerComponent(IPublishedElement block)
    {
        if (block is not Umbraco.Cms.Web.Common.PublishedModels.TextMediaBanner textMediaBannerBlock)
        {
            return null;
        }

        if (BannerTextMedia.Create(textMediaBannerBlock) is not { } textMediaBanner)
        {
            return null;
        }

        LayoutSection.CssClasses = textMediaBannerBlock.Theme != null ? $"t-{textMediaBannerBlock?.Theme?.Label}" : "t-white";
        LayoutSection.Variant = "in-grid";
        Id = textMediaBanner.Title.Trim().ToLower().Replace(" ", "-");

        return textMediaBanner;
    }
}
