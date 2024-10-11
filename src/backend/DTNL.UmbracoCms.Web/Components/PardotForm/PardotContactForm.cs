using DTNL.UmbracoCms.Web.Helpers.Aliases;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class PardotContactForm : PardotForm
{
    //TODO
    public override string ActionSubmitLabelKey => TranslationAliases.Common;

    public override string ActionSuccessLabelKey => TranslationAliases.Common;

    public override string ActionErrorLabelKey => TranslationAliases.Common;

    public required string[] InterestedIn { get; set; }

    public static PardotContactForm Create(NestedBlockContactForm contactForm)
    {
        return new()
        {
            ActionUrl = contactForm.PardotFormHandlerUrl!,
            InterestedIn = contactForm.InterestedIn.NotNullOrWhiteSpace().ToArray(),
        };
    }
}
