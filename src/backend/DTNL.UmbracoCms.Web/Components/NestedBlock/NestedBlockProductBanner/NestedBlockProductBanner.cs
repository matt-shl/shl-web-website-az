using Umbraco.Cms.Core.Models.PublishedContent;

namespace DTNL.UmbracoCms.Web.Components.NestedBlock;

public class NestedBlockProductBanner : NestedBlockWithInner
{
    protected override ProductDescription? GetInnerComponent(IPublishedElement block)
    {
        if (block is not Umbraco.Cms.Web.Common.PublishedModels.NestedBlockProductBanner productBannerBlock)
        {
            return null;
        }

        return ProductDescription.Create(productBannerBlock);
    }
}
