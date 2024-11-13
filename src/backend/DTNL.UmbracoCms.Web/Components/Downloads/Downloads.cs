using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class Downloads
{
    public string? Title { get; set; }

    public string? Description { get; set; }

    public required List<DownloadItem> Items { get; set; }

    public static Downloads? Create(NestedBlockDownloads downloads, SiteSettings? settings)
    {
        List<DownloadItem> downloadItems = downloads.Items
            .Using(block => DownloadItem.Create(block.Content, settings))
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
