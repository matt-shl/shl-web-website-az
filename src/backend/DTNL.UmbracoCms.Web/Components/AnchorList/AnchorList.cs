using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class AnchorList
{
    public bool ShowLinks { get; set; }

    public string? Alignment { get; set; }

    public string? CssClasses { get; set; }

    public bool IsComponent { get; set; }

    public required List<Link> Links { get; set; }

    public Button? LinkButton { get; set; }

    public static AnchorList Create(PageProduct productPage)
    {
        NestedBlockProductHero? hero = productPage.Hero
            .GetSingleContentOrNull<NestedBlockProductHero>();

        return new AnchorList
        {
            ShowLinks = hero is not null,
            LinkButton = Button
                .Create(hero?.PrimaryLink.GetSingleContentOrNull<NestedBlockButtonLink>())
                .With(b => b.Class = "anchor-list__cta"),
            IsComponent = true,
            Links = [],
        };
    }

    public static AnchorList Create(NestedBlockProductHero productHero)
    {
        return new AnchorList
        {
            ShowLinks = !productHero.HideAnchorLinks,
            Alignment = "vertical",
            CssClasses = "hero-pdp__anchor-links",
            IsComponent = false,
            Links = [],
        };
    }
}
