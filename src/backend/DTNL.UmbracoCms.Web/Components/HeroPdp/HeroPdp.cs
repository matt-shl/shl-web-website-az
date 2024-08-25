using DTNL.UmbracoCms.Web.Components.Hero;
using DTNL.UmbracoCms.Web.Helpers;
using DTNL.UmbracoCms.Web.Helpers.Aliases;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class HeroPdp : IHero
{
    public string? ThemeCssClasses { get; set; }

    public string? Title { get; set; }

    public string? Text { get; set; }

    public required ButtonLink? PrimaryLinkButton { get; set; }

    public Image? Image { get; set; }

    public required ButtonLink? SecondaryLinkButton { get; set; }

    public AnchorList? NavigationLinks { get; set; }

    public static HeroPdp? Create(NestedBlockProductHero? productHero, ICompositionContentBlocks page)
    {
        if (productHero is null)
        {
            return null;
        }

        return new HeroPdp
        {
            ThemeCssClasses = productHero.Theme is not null ? $"t-{productHero.Theme?.Label ?? "general"}" : ThemeHelper.GetCssClasses(page),

            Title = productHero.Title,

            Text = productHero.Text?.ToHtmlString(),

            PrimaryLinkButton = ButtonLink.Create(productHero.PrimaryLink?.FirstOrDefault(), cssClasses: "hero-pdp__cta1", svgIcon: SvgAliases.Icons.ArrowTopRight),

            Image = Image.Create(productHero.Image, cssClasses: "hero-pdp__image", style: "heroPdp"),

            SecondaryLinkButton = ButtonLink.Create(productHero.SecondaryLink?.FirstOrDefault(), cssClasses: "hero-pdp__cta2", svgIcon: SvgAliases.Icons.ArrowTopRight),

            NavigationLinks = productHero!.HideNavigationLinks
                ? null
                : AnchorList.Create(page)
                    .With(a =>
                    {
                        a.CssClasses = "hero-pdp__anchor-links";
                        a.Alignment = "vertical";
                    }),
        };
    }
}
