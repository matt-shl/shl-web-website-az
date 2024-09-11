using Umbraco.Cms.Core.Models.PublishedContent;

namespace DTNL.UmbracoCms.Web.Components.NestedBlock;

public class NestedBlockVideo : NestedBlock
{
    public required Video Video { get; set; }

    protected override object? ProcessBlock(IPublishedElement block)
    {
        if (block is not Umbraco.Cms.Web.Common.PublishedModels.NestedBlockVideo nestedBlockVideo)
        {
            return null;
        }

        if (Video.Create(nestedBlockVideo) is not { } video)
        {
            return null;
        }

        Video = video;

        return this;
    }
}
