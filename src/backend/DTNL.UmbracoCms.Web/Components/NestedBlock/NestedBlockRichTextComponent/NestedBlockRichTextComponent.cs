using Umbraco.Cms.Core.Models.PublishedContent;

namespace DTNL.UmbracoCms.Web.Components.NestedBlock;

public class NestedBlockRichTextComponent : NestedBlockWithInner
{
    protected override RichTextComponent? GetInnerComponent(IPublishedElement block)
    {
        if (block is not Umbraco.Cms.Web.Common.PublishedModels.NestedBlockRichText blockRichText)
        {
            return null;
        }

        LayoutSection.ReduceMargin = blockRichText.ReduceMargin;

        return RichTextComponent.Create(blockRichText, CultureDictionary);
    }
}
