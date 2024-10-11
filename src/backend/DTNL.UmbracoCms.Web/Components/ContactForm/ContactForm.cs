using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class ContactForm
{
    public string? Title { get; set; }

    public string? Text { get; set; }

    public required PardotContactForm PardotContactForm { get; set; }

    public static ContactForm Create(NestedBlockContactForm contactFormBlock)
    {
        return new ContactForm
        {
            Title = contactFormBlock.Title,
            Text = contactFormBlock.Text?.ToHtmlString(),
            PardotContactForm = PardotContactForm.Create(contactFormBlock),
        };
    }
}
