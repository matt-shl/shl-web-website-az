using DTNL.UmbracoCms.Web.Helpers.Aliases;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class PardotContactForm : PardotForm
{
    public override string ActionSubmitLabelKey => TranslationAliases.Forms.ContactForm.SubmitFormLabel;

    public override string ActionSuccessLabelKey => TranslationAliases.Forms.ContactForm.SubmissionSuccessMessage;

    public override string ActionErrorLabelKey => TranslationAliases.Forms.ContactForm.SubmissionErrorMessage;

    public static PardotContactForm? Create(NestedBlockContactForm contactForm)
    {
        if (contactForm.PardotFormHandlerUrl.IsNullOrWhiteSpace())
        {
            return null;
        }

        return new()
        {
            ActionUrl = contactForm.PardotFormHandlerUrl,
        };
    }
}
