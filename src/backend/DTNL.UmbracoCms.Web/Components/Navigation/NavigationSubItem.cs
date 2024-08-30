using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class NavigationSubItem
{
    public required string Id { get; set; }

    public required string Title { get; set; }

    public required Link Link { get; set; }

    public required List<Link> SubLinks { get; set; }

    public static NavigationSubItem Create(NestedBlockNavigationMenuSubItem navigationSubItem)
    {
        return new NavigationSubItem
        {
            Id = navigationSubItem.Key.ToString(),
            Title = navigationSubItem.Title!,
            Link = Link.Create(navigationSubItem.MainLink!),
            SubLinks = navigationSubItem.Sublinks
                .Using(l => Link.Create(l))
                .ToList(),
        };
    }
}
