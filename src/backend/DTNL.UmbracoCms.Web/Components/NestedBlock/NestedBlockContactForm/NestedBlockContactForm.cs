using Umbraco.Cms.Core.Models.PublishedContent;

namespace DTNL.UmbracoCms.Web.Components.NestedBlock;

public class NestedBlockContactForm : NestedBlockWithInner
{
    protected override ContactForm? GetInnerComponent(IPublishedElement block)
    {
        if (block is not Umbraco.Cms.Web.Common.PublishedModels.NestedBlockContactForm contactFormBlock)
        {
            return null;
        }

        LayoutSection.CssThemeClasses = "t-white";

        return ContactForm.Create(contactFormBlock);
    }
}
