using DTNL.UmbracoCms.Web.Helpers.Aliases;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class AnchorList
{
    public string? Alignment { get; set; }

    public string? CssClasses { get; set; }

    public bool IsComponent { get; set; }

    public required List<Link> Links { get; set; }

    public Button? LinkButton { get; set; }

    public static AnchorList? Create(IPublishedContent? content = null, bool isComponent = false)
    {
        ICompositionContentBlocks? block = (ICompositionContentBlocks?) content;

        NestedBlockProductHero? hero = null;
        if (content is PageProduct pageProduct)
        {
            hero = pageProduct.Hero?.FirstOrDefault()?.Content as NestedBlockProductHero;
        }

        List<Link>? links = block?.ContentBlocks
                       ?.Select(b => b.Content as ICompositionAnchors)
                       .Select(b => new Link { Label = b?.AnchorTitle, Url = $"#{b?.AnchorId}" })
                       .ToList();

        var anchorButtonlilnk = hero?.PrimaryLink?.FirstOrDefault()?.Content as NestedBlockButtonLink;

        if (isComponent)
        {

            return new AnchorList
            {
                LinkButton = Button.Create(anchorButtonlilnk?.Link)?.With(b =>
                {
                    b.Class = "anchor-list__cta";
                    b.Icon = SvgAliases.Icons.ArrowTopRight;
                    b.Variant = "primary";
                }),
                Alignment = "horizontal",
                IsComponent = isComponent,
                Links = links ?? [],
            };
        }
        else
        {
            return new AnchorList
            {
                IsComponent = isComponent,
                Links = links ?? [],
            };
        }
    }
}
