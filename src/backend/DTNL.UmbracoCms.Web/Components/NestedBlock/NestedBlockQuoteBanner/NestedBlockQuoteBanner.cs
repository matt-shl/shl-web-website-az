using Umbraco.Cms.Core.Models.PublishedContent;

namespace DTNL.UmbracoCms.Web.Components.NestedBlock;

public class NestedBlockQuoteBanner : NestedBlockWithInner
{
    protected override BannerQuote? GetInnerComponent(IPublishedElement block)
    {
        if (block is not Umbraco.Cms.Web.Common.PublishedModels.NestedBlockQuotesBanner quoteBannerBlock)
        {
            return null;
        }

        return BannerQuote.Create(quoteBannerBlock);
    }
}
