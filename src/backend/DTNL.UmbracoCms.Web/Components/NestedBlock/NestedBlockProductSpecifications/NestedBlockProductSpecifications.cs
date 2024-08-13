using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components.NestedBlock;

public class NestedBlockProductSpecifications : NestedBlockWithInner
{
    protected override DescriptionList? GetInnerComponent(IPublishedElement block)
    {
        if (block is not Umbraco.Cms.Web.Common.PublishedModels.NestedBlockProductSpecifications productSpecificationsBlock)
        {
            return null;
        }

        return DescriptionList.Create(productSpecificationsBlock, NodeProvider.CurrentNode as PageProduct);
    }
}
