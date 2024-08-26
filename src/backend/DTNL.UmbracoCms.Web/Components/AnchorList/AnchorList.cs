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

    public static AnchorList Create(ICompositionContentBlocks? block = null)
    {
        List<Link>? links = block?.ContentBlocks
                        ?.Select(b => b.Content as ICompositionAnchors)
                        .Select(b => new Link { Label = b?.AnchorTitle, Url = $"#{b?.AnchorId}" })
                        .ToList();
        return new AnchorList
        {
            IsComponent = block is null,
            Links = links ?? [],
        };
    }

    public static AnchorList? CreateforPage(IPublishedContent? content = null)
    {
        if (content is not ICompositionContentBlocks contentBlock)
        {
            return null;
        }

        if (content is not PageProduct pageProduct)
        {
            return null;
        }

        List<Link>? links = contentBlock.ContentBlocks
                       ?.Select(b => b.Content as ICompositionAnchors)
                       .Select(b => new Link { Label = b?.AnchorTitle, Url = $"#{b?.AnchorId}" })
                       .ToList();

        NestedBlockProductHero? hero = pageProduct.Hero?.FirstOrDefault()?.Content as NestedBlockProductHero;

        return new AnchorList
        {
            LinkButton = Button.Create(hero?.PrimaryLink?.FirstOrDefault())?.With(b =>
            {
                b.Class = "anchor-list__cta";
                b.Icon = SvgAliases.Icons.ArrowTopRight;
                b.Variant = "primary";
            }),
            Alignment = "horizontal",
            IsComponent = true,
            Links = links ?? [],

        };
    }
}
