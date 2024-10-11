using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class DownloadOverlay
{
    public required Modal Modal { get; set; }

    public static DownloadOverlay Create(NestedBlockDownloadItem downloadItem)
    {
        return new DownloadOverlay
        {
            Modal = new Modal
            {
                Id = $"modal-download-{downloadItem.Key}",
                Size = "aside-with-image",
                Title = downloadItem.DownloadItemTitle,
                SubTitle = downloadItem.DownloadItemDescription,
                Image = null,// TODO?
                KeepScrollPosition = true,
            },
        };
    }
}
