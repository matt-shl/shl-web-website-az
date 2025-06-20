using Umbraco.Cms.Core.Models.PublishedContent;

namespace DTNL.UmbracoCms.Web.Components.NestedBlock;

public class NestedBlockMap : NestedBlockWithInner
{
    protected override Map? GetInnerComponent(IPublishedElement block)
    {
        if (block is not Umbraco.Cms.Web.Common.PublishedModels.NestedBlockMap mapBlock)
        {
            return null;
        }

        LayoutSection.Variant = "no-padding";
        LayoutSection.CssThemeClasses = "t-general";

        return Map.Create(mapBlock);
    }
}
