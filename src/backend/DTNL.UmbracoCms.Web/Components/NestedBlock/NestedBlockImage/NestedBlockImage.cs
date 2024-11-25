using Umbraco.Cms.Core.Models.PublishedContent;

namespace DTNL.UmbracoCms.Web.Components.NestedBlock;

public class NestedBlockImage : NestedBlockWithInner
{
    protected override Media? GetInnerComponent(IPublishedElement block)
    {
        if (block is not Umbraco.Cms.Web.Common.PublishedModels.NestedBlockImage nestedBlockImage)
        {
            return null;
        }

        LayoutSection.CssClasses = "c-media-section-container";
        LayoutSection.CssThemeClasses = "t-white";
        LayoutSection.Variant = nestedBlockImage.FullWidth ? "no-padding" : "no-padding-inline-mobile";

        return Media.Create(nestedBlockImage.Image);
    }
}
