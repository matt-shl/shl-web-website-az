using DTNL.UmbracoCms.Web.Helpers.Aliases;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class DownloadOverlay
{
    public required Modal Modal { get; set; }

    public required PardotDownloadForm PardotDownloadForm { get; set; }

    public static DownloadOverlay? Create(NestedBlockDownloadItem downloadItem, SiteSettings? settings)
    {
        if (PardotDownloadForm.Create(downloadItem) is not { } pardotDownloadForm)
        {
            return null;
        }

        Image? image = Image
            .Create(settings?.DownloadFormImage ?? downloadItem.File, style: "modal");

        return new DownloadOverlay
        {
            Modal = new Modal
            {
                Id = $"modal-download-{downloadItem.Key}",
                Size = image is null ? "aside" : "aside-with-image",
                Title = TranslationAliases.Forms.DownloadForm.Title,
                SubTitle = downloadItem.Title,
                Image = image,
                KeepScrollPosition = true,
            },
            PardotDownloadForm = pardotDownloadForm,
        };
    }
}
