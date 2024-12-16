namespace DTNL.UmbracoCms.Web.Components;

public class NavigationMobileList
{
    public string? Id { get; set; }

    public string? Name { get; set; }

    public string? ParentId { get; set; }

    public List<NavigationItem>? NavigationItems { get; set; }

    public List<NavigationSubItem>? NavigationSubItems { get; set; }

    public List<Link>? NavigationSubItemLinks { get; set; }

    public Link? Link { get; set; }

    public Link? NavigationItemLink { get; set; }
}
