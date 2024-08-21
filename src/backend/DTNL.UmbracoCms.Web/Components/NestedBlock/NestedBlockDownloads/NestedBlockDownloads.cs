using Umbraco.Cms.Core.Models.PublishedContent;

namespace DTNL.UmbracoCms.Web.Components.NestedBlock;

public class NestedBlockDownloads : NestedBlockWithInner
{
    public required Downloads Downloads { get; set; }

    protected override Downloads? GetInnerComponent(IPublishedElement block)
    {
        if (block is not Umbraco.Cms.Web.Common.PublishedModels.NestedBlockDownloads downloadsBlock)
        {
            return null;
        }

        LayoutSection.CssThemeClasses = "t-white";
        LayoutSection.Id = downloadsBlock.AnchorId;
        LayoutSection.NavigationTitle = downloadsBlock.AnchorTitle;

        return Downloads.Create(downloadsBlock);
    }
}
