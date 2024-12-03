using DTNL.UmbracoCms.Web.Helpers.Aliases;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using DTNL.UmbracoCms.Web.Models.BrandfolderAssets;
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
                    .Using(l => l.Content as NestedBlockTextMediaListItem)
                    .Using(l => Link
                        .Create(l.Link, cssClasses: "link-list__anchor")
                        .With(link =>
                        {
                            link.IconPath = BrandfolderAttachment.GetAssetUrl(l.Icon);
                            link.IconPath ??= SvgAliases.Icons.ArrowTopRight;
                        }))
                .ToList(),
                CssClasses = "text-media-list__link-list",
            };
    }
}
