using DTNL.UmbracoCms.Web.Helpers.Aliases;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class AnchorList
{
    public string? Alignment { get; set; }

    public string? CssClasses { get; set; }

    public bool IsComponent { get; set; }

    public required List<Link> Links { get; set; }

    public required Button? LinkButton { get; set; }

    public static AnchorList Create(NestedBlockLinks? linksBlock = null)
    {
        return new AnchorList
        {
            IsComponent = linksBlock is null,
            Links = (linksBlock?.Links)
                .Using(link => Link.Create(link))
                .ToList(),
            LinkButton = Button.Create((Link?) null)
                .With(b =>
                {
                    b.Class = "anchor-list__cta";
                    b.Icon = SvgAliases.Icons.ArrowTopRight;
                }),
        };
    }
}
