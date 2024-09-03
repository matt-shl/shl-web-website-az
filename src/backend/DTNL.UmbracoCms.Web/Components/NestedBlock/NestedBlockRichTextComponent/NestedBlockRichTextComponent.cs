using Umbraco.Cms.Core.Models.PublishedContent;

namespace DTNL.UmbracoCms.Web.Components.NestedBlock;

public class NestedBlockRichTextComponent : NestedBlockWithInner
{
    protected override RichTextComponent? GetInnerComponent(IPublishedElement block)
    {
        if (block is not Umbraco.Cms.Web.Common.PublishedModels.NestedBlockRichTextComponent richTextComponent)
        {
            return null;
        }

        LayoutSection.CssClasses = "t-white";
        LayoutSection.Variant = "reduce-margin-bottom";

        return RichTextComponent.Create(richTextComponent, CultureDictionary);
    }
}
