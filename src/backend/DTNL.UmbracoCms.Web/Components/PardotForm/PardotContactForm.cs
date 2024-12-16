using DTNL.UmbracoCms.Web.Helpers.Aliases;

namespace DTNL.UmbracoCms.Web.Components;

public class PardotContactForm : PardotForm
{
    public override string ActionSubmitLabelKey => TranslationAliases.Forms.ContactForm.SubmitFormLabel;

    public override string ActionSuccessLabelKey => TranslationAliases.Forms.ContactForm.SubmissionSuccessMessage;

    public override string ActionErrorLabelKey => TranslationAliases.Forms.ContactForm.SubmissionErrorMessage;

    public override string? ConsentFieldName => "cf_Consent Given";

    public static PardotContactForm Create()
    {
        return new()
        {
            Id = Guid.NewGuid().ToString(),
            ActionUrl = "https://go.shl-medical.com/l/1046193/2024-08-20/h845",
            Attributes = new()
            {
                ["gtm"] = "{'event': 'contact_form'}",
            },
        };
    }
}
