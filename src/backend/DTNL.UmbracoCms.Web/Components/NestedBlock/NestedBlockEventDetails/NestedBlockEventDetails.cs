using Umbraco.Cms.Core.Models.PublishedContent;

namespace DTNL.UmbracoCms.Web.Components.NestedBlock;

public class NestedBlockEventDetails : NestedBlock
{
    protected override object? ProcessBlock(IPublishedElement block)
    {
        if (block is not Umbraco.Cms.Web.Common.PublishedModels.NestedBlockEventDetails nestedBlockEventDetails)
        {
            return null;
        }

        return EventDetails.Create(nestedBlockEventDetails, NodeProvider.SiteSettings);
    }
}
