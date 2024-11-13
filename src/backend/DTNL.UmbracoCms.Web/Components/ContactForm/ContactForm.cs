using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class ContactForm
{
    public string? Title { get; set; }

    public string? Text { get; set; }

    public required List<ContactFormItem> Items { get; set; }

    public PardotContactForm? PardotContactForm { get; set; }

    public static ContactForm Create(NestedBlockContactForm contactFormBlock)
    {
        return new ContactForm
        {
            Title = contactFormBlock.Title,
            Text = contactFormBlock.Text?.ToHtmlString(),
            Items = contactFormBlock.Items
                .Using(item => item.Content as NestedBlockContactFormItem)
                .Using(ContactFormItem.Create)
                .ToList(),
            PardotContactForm = PardotContactForm.Create(contactFormBlock),
        };
    }
}
