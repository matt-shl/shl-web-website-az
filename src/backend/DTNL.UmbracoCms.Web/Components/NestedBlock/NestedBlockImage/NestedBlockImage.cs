using Umbraco.Cms.Core.Models.PublishedContent;

namespace DTNL.UmbracoCms.Web.Components.NestedBlock;

public class NestedBlockMedia : NestedBlockWithInner
{
    protected override Media? GetInnerComponent(IPublishedElement block)
    {
        if (block is not Umbraco.Cms.Web.Common.PublishedModels.NestedBlockImage imageBlock)
        {
            return null;
        }

        LayoutSection.CssClasses = "c-media-section-container";
        LayoutSection.CssThemeClasses = "t-white";
        LayoutSection.Variant = "no-padding-inline-mobile";

        return Media.Create(imageBlock.Image);
    }
}
