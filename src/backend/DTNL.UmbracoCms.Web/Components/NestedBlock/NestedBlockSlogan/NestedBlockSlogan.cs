using Umbraco.Cms.Core.Models.PublishedContent;

namespace DTNL.UmbracoCms.Web.Components.NestedBlock;

public class NestedBlockSlogan : NestedBlock
{
    public required Slogan Slogan { get; set; }

    protected override object? ProcessBlock(IPublishedElement block)
    {
        if (block is not Umbraco.Cms.Web.Common.PublishedModels.NestedBlockSlogan nestedBlockSlogan)
        {
            return null;
        }

        Slogan? slogan = Slogan.Create(nestedBlockSlogan);
        if (slogan == null)
        {
            return null;
        }

        Slogan = slogan;

        return this;
    }
}
