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
        LayoutSection.ReduceMargin = richTextComponent.ReduceMargin;
        LayoutSection.Id = richTextComponent.AnchorId;
        LayoutSection.NavigationTitle = richTextComponent.AnchorTitle;

        return RichTextComponent.Create(richTextComponent, CultureDictionary);
    }
}
