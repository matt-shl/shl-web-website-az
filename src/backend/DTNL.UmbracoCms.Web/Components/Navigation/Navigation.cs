using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class Navigation
{
    public required List<NavigationItem> Items { get; set; }

    public required FlyoutSearch SearchFlyout { get; set; }

    public static Navigation Create(ICompositionHeader? navigation)
    {
        return new Navigation
        {
            Items = (navigation?.MainNavigationItems)
                    .OrEmptyIfNull()
                    .Select(blockListItem => blockListItem.Content)
                    .Using(NavigationItem.Create)
                    .ToList(),
            SearchFlyout = FlyoutSearch.Create("search"),
        };
    }
}
