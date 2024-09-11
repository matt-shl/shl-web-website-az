using Umbraco.Cms.Core.Models.PublishedContent;

namespace DTNL.UmbracoCms.Web.Components.NestedBlock.NestedBlockTextMediaList;

public class NestedBlockTextMediaList : NestedBlockWithInner
{
    protected override TextMediaList? GetInnerComponent(IPublishedElement block)
    {
        if (block is not Umbraco.Cms.Web.Common.PublishedModels.NestedBlockTextMediaList textMediaListBlock)
        {
            return null;
        }

        return TextMediaList.Create(textMediaListBlock);
    }
}
