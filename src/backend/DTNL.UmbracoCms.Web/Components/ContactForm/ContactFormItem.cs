using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class ContactFormItem
{
    public string? Title { get; set; }

    public string? SubTitle { get; set; }

    public string? Text { get; set; }

    public string? IconPath { get; set; }

    public static ContactFormItem Create(NestedBlockContactFormItem contactFormBlockItem)
    {
        return new ContactFormItem
        {
            Title = contactFormBlockItem.Title,
            Text = contactFormBlockItem.Text?.ToHtmlString(),
            IconPath = contactFormBlockItem.Icon?.LocalCrops.Src,
        };
    }
}
