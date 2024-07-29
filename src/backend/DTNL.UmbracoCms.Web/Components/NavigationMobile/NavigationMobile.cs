using Umbraco.Cms.Web.Common.PublishedModels;
using static DTNL.UmbracoCms.Web.Components.NavigationDesktop;

namespace DTNL.UmbracoCms.Web.Components;

public class NavigationMobile
{
    public IEnumerable<MenuItem> Items { get; set; } = new List<MenuItem>();
    public NavigationHeaderListItem? Header { get; set; }
    public NavigationListItem? Parent { get; set; }
    public Link? AllLink { get; set; }

    public class NavigationListItem
    {
        public required string? Title { get; set; }

        public int? Id { get; set; }
    }

    public class NavigationHeaderListItem
    {
        public required string? Title { get; set; }

        public string? Id { get; set; }
    }



    public static NavigationMobile Create(NestedBlockNavigation? navigation)
    {
        if (navigation == null || navigation.Items is not { } navigationItems)
        {
            return new NavigationMobile
            {
                Items = [],
            };
        }

        return new NavigationMobile
        {
            Items = navigation.Items
                    .Select(i => i.Content)
                    .OfType<NestedBlockHeaderLink>()
                    .Select((m, i) => m.MainLink != null
                        ? new MenuItem { Id = i, Title = m.MainLink.Name, Description = m.Description, Link = Link.Create(m.MainLink), Submenu = BuildMenuLevel2(m, i), }
                        : null)
                    .WhereNotNull().ToList(),
        };
    }
}
