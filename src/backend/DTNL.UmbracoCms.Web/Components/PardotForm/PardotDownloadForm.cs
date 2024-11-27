using DTNL.UmbracoCms.Web.Helpers.Aliases;
using Flurl;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class PardotDownloadForm : PardotForm
{
    public override string ActionSubmitLabelKey => TranslationAliases.Forms.EventForm.SubmitFormLabel;

    public override string ActionSuccessLabelKey => TranslationAliases.Forms.EventForm.SubmissionSuccessMessage;

    public override string ActionErrorLabelKey => TranslationAliases.Forms.EventForm.SubmissionErrorMessage;

    public override string ConsentFieldName => "cf_Consent Given";

    public required string FileUrl { get; set; }

    public static PardotDownloadForm? Create(NestedBlockDownloadItem downloadItem)
    {
        if (string.IsNullOrWhiteSpace(downloadItem.File))
        {
            return null;
        }

        return new()
        {
            Id = Guid.NewGuid().ToString(),
            ActionUrl = "http://go.shl-medical.com/l/1046193/2024-11-08/nrq8",
            FileUrl = downloadItem.File.RemoveQuery(),
        };
    }
}
