using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace DTNL.UmbracoCms.Web.Components;

public class Breadcrumb : ViewComponentExtended
{
    public required List<Link> Pages { get; set; }

    public IViewComponentResult Invoke()
    {
        Pages = (NodeProvider.GetCurrentNode()?
            .AncestorsOrSelf())
            .OrEmptyIfNull()
            .Where(ancestor => !ancestor.IsFolder())
            .OrderBy(x => x.Level)
            .Select(l => Link.Create(l, false))
            .WhereNotNull()
            .ToList();

        return View("Breadcrumb", this);
    }
}
