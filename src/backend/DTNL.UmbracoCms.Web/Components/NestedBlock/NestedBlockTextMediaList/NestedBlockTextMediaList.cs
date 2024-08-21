using Umbraco.Cms.Core.Models.PublishedContent;

namespace DTNL.UmbracoCms.Web.Components.NestedBlock.NestedBlockTextMediaList;

public class NestedBlockTextMediaList : NestedBlockWithInner
{
    protected override TextMediaList? GetInnerComponent(IPublishedElement block)
    {
        if (block is not Umbraco.Cms.Web.Common.PublishedModels.NestedBlockTextMediaList textMediaListBlock)
        {
            return null;
        }

        if (TextMediaList.Create(textMediaListBlock) is not { } textMediaList)
        {
            return null;
        }

        LayoutSection.CssClasses = textMediaListBlock.Theme != null ? $"t-{textMediaListBlock?.Theme?.Label}" : "t-white";
        LayoutSection.Variant = "in-grid";
        LayoutSection.Id = textMediaList.AnchorId;
        LayoutSection.NavigationTitle = textMediaList.AnchorTitle;

        return textMediaList;
    }
}
