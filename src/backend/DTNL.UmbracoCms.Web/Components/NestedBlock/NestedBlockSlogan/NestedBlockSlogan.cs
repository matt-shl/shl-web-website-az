using Umbraco.Cms.Core.Models.PublishedContent;

namespace DTNL.UmbracoCms.Web.Components.NestedBlock;

public class NestedBlockSlogan : NestedBlockWithInner
{
    protected override object? GetInnerComponent(IPublishedElement block)
    {
        if (block is not Umbraco.Cms.Web.Common.PublishedModels.NestedBlockSlogan nestedBlockSlogan)
        {
            return null;
        }

        if (Slogan.Create(nestedBlockSlogan) is not { } slogan)
        {
            return null;
        }

        LayoutSection.CssContainerClasses = "styleguide__component-col styleguide__component-col--theme styleguide__component-col--12";
        LayoutSection.CssThemeClasses = "t-white";

        return slogan;
    }
}
