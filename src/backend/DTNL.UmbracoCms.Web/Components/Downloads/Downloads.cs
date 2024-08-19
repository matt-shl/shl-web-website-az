using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class Downloads
{
    public string? Title { get; set; }

    public string? Description { get; set; }

    public IEnumerable<DownloadItem?>? Items { get; set; }

    public static Downloads? Create(NestedBlockDownloads downloads)
    {
        if (downloads == null)
        {
            return null;
        }

        return new Downloads
        {
            Title = downloads.DownloadTitle,
            Description = downloads.DownloadsSubtitle,
            Items = downloads.DownloadsList?.Select(block => DownloadItem.Create(block.Content)),
        };
    }
}
