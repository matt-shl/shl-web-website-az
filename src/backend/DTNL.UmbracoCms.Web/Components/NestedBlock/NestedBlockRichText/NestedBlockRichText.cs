using Umbraco.Cms.Core.Models.PublishedContent;

namespace DTNL.UmbracoCms.Web.Components.NestedBlock;

public class NestedBlockRichText : NestedBlockWithInner
{
    protected override RichText? GetInnerComponent(IPublishedElement block)
    {
        if (block is not Umbraco.Cms.Web.Common.PublishedModels.NestedBlockRichText blockRichText)
        {
            return null;
        }

        LayoutSection.ReduceMargin = blockRichText.ReduceMargin;

        return RichText.Create(blockRichText, CultureDictionary);
    }
}
