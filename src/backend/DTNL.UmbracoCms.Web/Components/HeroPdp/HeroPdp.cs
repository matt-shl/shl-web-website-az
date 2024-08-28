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

    public required Button? PrimaryLinkButton { get; set; }

    public Image? Image { get; set; }

    public required Button? SecondaryLinkButton { get; set; }

    public AnchorList? NavigationLinks { get; set; }

    public static HeroPdp? Create(NestedBlockProductHero? productHero, ICompositionContentBlocks page)
    {
        if (productHero is null)
        {
            return null;
        }

        NestedBlockButtonLink? primaryButtonLink = productHero.PrimaryLink?.FirstOrDefault()?.Content as NestedBlockButtonLink;
        NestedBlockButtonLink? secondaryButtonLink = productHero.SecondaryLink?.FirstOrDefault()?.Content as NestedBlockButtonLink;

        return new HeroPdp
        {
            ThemeCssClasses = productHero.Theme is not null ? $"t-{productHero.Theme?.Label ?? "general"}" : ThemeHelper.GetCssClasses(page),

            Title = productHero.Title,

            Text = productHero.Text?.ToHtmlString(),

            PrimaryLinkButton = Button.Create(primaryButtonLink?.Link).With(b =>
            {
                b.Class = "hero-pdp__cta1";
                b.Icon = primaryButtonLink?.ButtonIcon?.LocalCrops.Src ?? SvgAliases.Icons.ArrowTopRight;
                b.Variant = primaryButtonLink?.Variant;
            }),

            Image = Image.Create(productHero.Image, cssClasses: "hero-pdp__image", style: "heroPdp"),

            SecondaryLinkButton = Button.Create(secondaryButtonLink?.Link).With(b =>
            {
                b.Class = "hero-pdp__cta2";
                b.Icon = secondaryButtonLink?.ButtonIcon?.LocalCrops.Src ?? SvgAliases.Icons.ArrowTopRight;
                b.Variant = secondaryButtonLink?.Variant;
            }),

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
