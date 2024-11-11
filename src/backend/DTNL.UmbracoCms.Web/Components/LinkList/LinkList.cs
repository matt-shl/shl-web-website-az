using DTNL.UmbracoCms.Web.Helpers.Aliases;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class LinkList
{
    public required List<Link> Links { get; set; }

    public string? CssClasses { get; set; }

    public static LinkList? Create(NestedBlockTextMediaListLinks? textMediaListLinks)
    {
        return textMediaListLinks is null
            ? null
            : new LinkList
            {
                Links = textMediaListLinks.Links
                .Using(l => Link.Create(((NestedBlockTextMediaListItem) l.Content).Link, cssClasses: "link-list__anchor").With(link =>
                {
                    link.IconPath = ((NestedBlockTextMediaListItem) l.Content)?.Icon?.LocalCrops.Src ?? SvgAliases.Icons.ArrowTopRight;
                }))
                .ToList(),
                CssClasses = "text-media-list__link-list",
            };
    }
}
