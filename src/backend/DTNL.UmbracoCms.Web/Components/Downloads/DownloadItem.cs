using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class DownloadItem
{
    public required string Title { get; set; }

    public string? Description { get; set; }

    public string? Icon { get; set; }

    public required DownloadOverlay DownloadOverlay { get; set; }

    public static DownloadItem? Create(IPublishedElement item)
    {
        if (item is not NestedBlockDownloadItem downloadItem)
        {
            return null;
        }

        return new DownloadItem
        {
            Title = downloadItem.DownloadItemTitle!,
            Description = downloadItem.DownloadItemDescription,
            Icon = downloadItem.DownloadIcon?.LocalCrops.Src,
            DownloadOverlay = DownloadOverlay.Create(downloadItem),
        };
    }
}
