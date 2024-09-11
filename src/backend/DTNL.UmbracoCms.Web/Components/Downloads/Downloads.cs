using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class Downloads
{
    public string? Title { get; set; }

    public string? Description { get; set; }

    public required List<DownloadItem> Items { get; set; }

    public static Downloads? Create(NestedBlockDownloads downloads)
    {
        List<DownloadItem> downloadItems = downloads.DownloadsList
            .Using(block => DownloadItem.Create(block.Content))
            .ToList();

        if (downloadItems.Count == 0)
        {
            return null;
        }

        return new Downloads
        {
            Title = downloads.Title,
            Description = downloads.SubTitle,
            Items = downloadItems,
        };
    }
}
