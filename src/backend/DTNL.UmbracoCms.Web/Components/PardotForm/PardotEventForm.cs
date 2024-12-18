using DTNL.UmbracoCms.Web.Helpers.Aliases;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class PardotEventForm : PardotForm
{
    public override string ActionSubmitLabelKey => TranslationAliases.Forms.EventForm.SubmitFormLabel;

    public override string ActionSuccessLabelKey => TranslationAliases.Forms.EventForm.SubmissionSuccessMessage;

    public override string ActionErrorLabelKey => TranslationAliases.Forms.EventForm.SubmissionErrorMessage;

    public static PardotEventForm Create(NestedBlockEventDetails eventDetails)
    {
        return new()
        {
            Id = Guid.NewGuid().ToString(),
            ActionUrl = "https://go.shl-medical.com/l/1046193/2024-10-30/my2k",
            GtmAttributes = $"{{'event': 'register_event','option_clicked': '{eventDetails.EventTitle}'}}",
        };
    }
}
