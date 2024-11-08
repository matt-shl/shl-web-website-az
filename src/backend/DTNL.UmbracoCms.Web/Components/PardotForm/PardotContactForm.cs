using DTNL.UmbracoCms.Web.Helpers.Aliases;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class PardotContactForm : PardotForm
{
    public override string ActionSubmitLabelKey => TranslationAliases.Forms.ContactForm.SubmitFormLabel;

    public override string ActionSuccessLabelKey => TranslationAliases.Forms.ContactForm.SubmissionSuccessMessage;

    public override string ActionErrorLabelKey => TranslationAliases.Forms.ContactForm.SubmissionErrorMessage;

    public override string? ConsentFieldName => "cf_Consent Given";

    public static PardotContactForm Create(NestedBlockContactForm contactForm)
    {
        return new()
        {
            Id = Guid.NewGuid().ToString(),
            ActionUrl = "https://go.shl-medical.com/l/1046193/2024-02-07/3kcc",
        };
    }
}
