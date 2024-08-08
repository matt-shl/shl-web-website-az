using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Web.Common.PublishedModels;
using static DTNL.UmbracoCms.Web.Components.Navigation;
using DTNL.UmbracoCms.Web.Helpers.Aliases;

namespace DTNL.UmbracoCms.Web.Components;

public class Navigation
{
    public IEnumerable<MenuItem> Items { get; set; } = new List<MenuItem>();

    public NavigationHeaderListItem? Header { get; set; }

    public NavigationListItem? Parent { get; set; }

    public Link? AllLink { get; set; }

    public class MenuItem
    {
        public required int Id { get; set; }

        public Link? Link { get; set; }

        public Submenu? Submenu { get; set; }

        public NestedSubmenu? NestedSubmenu { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public Link? All { get; set; }

        public CardImage? PromoBlock { get; set; }

        public NavigationHeaderListItem? Header { get; set; }

        public NavigationListItem? Parent { get; set; }
    }

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

    public class Submenu
    {
        public required int Id { get; set; }

        public string? Title { get; set; }

        public required IEnumerable<MenuItem> Items { get; set; }
    }

    public class NestedSubmenu
    {
        public required int Id { get; set; }

        public string? Title { get; set; }

        public required IEnumerable<MenuItem> Links { get; set; }

    }

    //public class PromoFeature
    //{
    //    public string? Title { get; set; }

    //    public string? Content { get; set; }

    //    public Link? Url { get; set; }

    //    public MediaWithCrops? Image { get; set; }

    //}

    public static Navigation Create(NestedBlockNavigation? navigation)
    {
        if (navigation == null || navigation.Items is not { } navigationItems)
        {
            return new Navigation
            {
                Items = [],
            };
        }

        return new Navigation
        {
            Items = navigation.Items
                    .Select(i => i.Content)
                    .OfType<NestedBlockHeaderLink>()
                    .Select((m, i) => m.MainLink != null
                        ? new MenuItem { Id = i, Title = m.MainLink.Name, Description = m.Description, Link = Link.Create(m.MainLink), Submenu = BuildMenuLevel2(m, i), PromoBlock = BuildPromoBlock(m) }
                        : null)
                    .WhereNotNull().ToList(),

        };
    }

    public static Submenu? BuildMenuLevel2(NestedBlockHeaderLink headerLink, int index)
    {
        if (headerLink == null || headerLink.SubLinks == null)
        {
            return null;
        }

        return new Submenu
        {
            Id = index,
            Title = headerLink.MainLink?.Name,
            Items = headerLink.SubLinks.Select(i => i.Content)
                    .OfType<NestedBlockHeaderSubLink>().Select((m, i) => m.MainLink != null
                        ? new MenuItem { Id = i, Title = m.MainLink.Name, All = Link.Create(m.MainLink), NestedSubmenu = BuildMenuLevel3(m, i) }
                        : null)
                    .WhereNotNull().ToList(),


            //PromoBlock = subMenu.PromoBlock?.FirstOrDefault()?.Content as NestedBlockPromoBlock,
        };
    }

    private static NestedSubmenu? BuildMenuLevel3(NestedBlockHeaderSubLink headerSubLink, int index)
    {
        if (headerSubLink.Menu?.FirstOrDefault()?.Content is not NestedBlockHeaderSubMenu subMenu)
        {
            return null;
        }

        return new NestedSubmenu
        {
            Id = index,
            Title = headerSubLink.MainLink?.Name,
            Links = subMenu.Links != null
            ? subMenu.Links.Select((m, i) => m != null ? new MenuItem { Id = i, Title = m.Name, Link = Link.Create(m) } : null).WhereNotNull().ToList()
            : []
        };
    }

    private static CardImage? BuildPromoBlock(NestedBlockHeaderLink headerLink)
    {
        if (headerLink.Promo?.FirstOrDefault()?.Content is not NestedBlockImageCard promoFeature)
        {
            return null;
        }

        return CardImage.Create(promoFeature);
    }
}
