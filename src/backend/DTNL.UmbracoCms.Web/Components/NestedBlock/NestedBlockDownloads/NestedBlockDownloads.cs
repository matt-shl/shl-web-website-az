using Umbraco.Cms.Core.Models.PublishedContent;

namespace DTNL.UmbracoCms.Web.Components.NestedBlock;

public class NestedBlockDownloads : NestedBlockWithInner
{
    protected override Downloads? GetInnerComponent(IPublishedElement block)
    {
        if (block is not Umbraco.Cms.Web.Common.PublishedModels.NestedBlockDownloads downloadsBlock)
        {
            return null;
        }

        return Downloads.Create(downloadsBlock);
    }
}
