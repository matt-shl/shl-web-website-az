using DTNL.UmbracoCms.Web.Helpers.Aliases;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class PardotEventForm : PardotForm
{
    public override string ActionSubmitLabelKey => TranslationAliases.Forms.EventForm.SubmitFormLabel;

    public override string ActionSuccessLabelKey => TranslationAliases.Forms.EventForm.SubmissionSuccessMessage;

    public override string ActionErrorLabelKey => TranslationAliases.Forms.EventForm.SubmissionErrorMessage;

    public static PardotEventForm? Create(NestedBlockEventDetails eventDetails)
    {
        if (eventDetails.EventPardotFormHandlerUrl.IsNullOrWhiteSpace())
        {
            return null;
        }

        return new()
        {
            ActionUrl = eventDetails.EventPardotFormHandlerUrl,
        };
    }
}
