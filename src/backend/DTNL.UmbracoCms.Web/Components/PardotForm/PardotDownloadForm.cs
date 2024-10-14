using DTNL.UmbracoCms.Web.Helpers.Aliases;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class PardotDownloadForm : PardotForm
{
    public override string ActionSubmitLabelKey => TranslationAliases.Forms.EventForm.SubmitFormLabel;

    public override string ActionSuccessLabelKey => TranslationAliases.Forms.EventForm.SubmissionSuccessMessage;

    public override string ActionErrorLabelKey => TranslationAliases.Forms.EventForm.SubmissionErrorMessage;

    public required string FileUrl { get; set; }

    public static PardotDownloadForm? Create(NestedBlockDownloadItem downloadItem)
    {
        if (!string.IsNullOrWhiteSpace(downloadItem.Link?.Url))
        {
            return null;
        }

        return new()
        {
            ActionUrl = "http://go.shl-medical.com/l/1046193/2024-02-07/3kcc",
            FileUrl = downloadItem.Link!.Url!,
        };
    }
}
