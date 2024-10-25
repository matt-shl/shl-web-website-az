using Umbraco.Cms.Core.Models.PublishedContent;

namespace DTNL.UmbracoCms.Web.Components.NestedBlock;

public class NestedBlockVideo : NestedBlockWithInner
{
    public required Media Video { get; set; }

    protected override object? GetInnerComponent(IPublishedElement block)
    {
        if (block is not Umbraco.Cms.Web.Common.PublishedModels.NestedBlockVideo nestedBlockVideo)
        {
            return null;
        }

        LayoutSection.CssClasses = "c-media-section-container";
        LayoutSection.CssThemeClasses = "t-white";
        LayoutSection.Variant = nestedBlockVideo.FullWidth ? "no-padding" : "no-padding-inline-mobile";

        return Media.Create(nestedBlockVideo);
    }
}
