using Umbraco.Cms.Core.Models.PublishedContent;

namespace DTNL.UmbracoCms.Web.Components.NestedBlock;

public class NestedBlockDownloads : NestedBlockWithInner
{
    public required Downloads Downloads { get; set; }

    protected override object? ProcessBlock(IPublishedElement block)
    {
        if (block is not Umbraco.Cms.Web.Common.PublishedModels.NestedBlockDownloads downloads)
        {
            return null;
        }

        return Downloads.Create(downloads);
    }
    protected override Downloads? GetInnerComponent(IPublishedElement block)
    {
        if (block is not Umbraco.Cms.Web.Common.PublishedModels.NestedBlockDownloads downloadsBlock)
        {
            return null;
        }
        return Downloads.Create(downloadsBlock);
    }
}
