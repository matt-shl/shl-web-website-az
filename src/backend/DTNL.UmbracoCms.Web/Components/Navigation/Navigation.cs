using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class Navigation
{
    public required List<NavigationItem> Items { get; set; }

    public static Navigation Create(ICompositionHeader? navigation)
    {
        if (navigation is not { MainNavigationItems.Count: > 0 })
        {
            return new Navigation
            {
                Items = [],
            };
        }

        return new Navigation
        {
            Items = navigation.MainNavigationItems
                    .Select(blockListItem => blockListItem.Content)
                    .Using(NavigationItem.Create)
                    .ToList(),
        };
    }
}
