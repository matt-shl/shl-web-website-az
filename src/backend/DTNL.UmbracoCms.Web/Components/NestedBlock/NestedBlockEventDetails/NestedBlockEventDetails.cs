using Umbraco.Cms.Core.Models.PublishedContent;

namespace DTNL.UmbracoCms.Web.Components.NestedBlock;

public class NestedBlockEventDetails : NestedBlockWithInner
{
    protected override object? GetInnerComponent(IPublishedElement block)
    {
        if (block is not Umbraco.Cms.Web.Common.PublishedModels.NestedBlockEventDetails nestedBlockEventDetails)
        {
            return null;
        }

        if (EventDetails.Create(nestedBlockEventDetails) is not { } slogan)
        {
            return null;
        }

        LayoutSection.CssContainerClasses = "asd";

        return slogan;
    }
}
