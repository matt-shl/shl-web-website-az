using Umbraco.Cms.Core.Models.PublishedContent;

namespace DTNL.UmbracoCms.Web.Components.NestedBlock;

public class NestedBlockSlogan : NestedBlock
{
    protected override object? ProcessBlock(IPublishedElement block)
    {
        if (block is not Umbraco.Cms.Web.Common.PublishedModels.NestedBlockSlogan nestedBlockSlogan)
        {
            return null;
        }

        if (Slogan.Create(nestedBlockSlogan) is not { } slogan)
        {
            return null;
        }

        return slogan;
    }
}
