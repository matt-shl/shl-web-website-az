using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class NavigationItem
{
    public required string Id { get; set; }

    public required Link Link { get; set; }

    public required string Title { get; set; }

    public string? Text { get; set; }

    public required List<NavigationSubItem> SubItems { get; set; }

    public CardImage? HighlightedCard { get; set; }

    public static NavigationItem? Create(IPublishedElement navigationItem)
    {
        return navigationItem switch
        {
            NestedBlockNavigationLink navigationLink => new()
            {
                Id = navigationLink.Key.ToString(),
                Link = Link.Create(navigationLink.Link!),
                Title = navigationLink.Link!.Name!,
                SubItems = [],
            },
            NestedBlockNavigationMenuItem navigationMenuItem => new()
            {
                Id = navigationMenuItem.Key.ToString(),
                Link = Link.Create(navigationMenuItem.Link!),
                Title = navigationMenuItem.Title!,
                Text = navigationMenuItem.Text,
                SubItems = navigationMenuItem.SubItems
                    .Using(blockListItem => blockListItem.Content as NestedBlockNavigationMenuSubItem)
                    .Using(NavigationSubItem.Create)
                    .ToList(),
                HighlightedCard =
                    CardImage.Create(
                        navigationMenuItem.HighlightedCard.GetSingleContentOrNull<NestedBlockImageCard>()),
            },
            _ => null,
        };
    }
}
