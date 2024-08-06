using DTNL.UmbracoCms.Web.Components.Hero;
using DTNL.UmbracoCms.Web.Helpers.Aliases;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class HeroPdp : IHero
{
    public string? Theme { get; set; }

    public string? Title { get; set; }

    public string? Text { get; set; }

    public required Button? PrimaryLinkButton { get; set; }

    public Image? Image { get; set; }

    public required Button? SecondaryLinkButton { get; set; }

    public AnchorList? NavigationLinks { get; set; }

    public static HeroPdp? Create(NestedBlockProductHero? productHero)
    {
        if (productHero is null)
        {
            return null;
        }

        return new HeroPdp
        {
            Theme = productHero.Theme?.Label, // TODO fix this upon merge of themes PR
            Title = productHero.Title,
            Text = productHero.Text?.ToHtmlString(),
            PrimaryLinkButton = Button.Create(productHero.PrimaryLink)
                .With(b =>
                {
                    b.Class = "hero-pdp__cta1";
                    b.Icon = SvgAliases.Icons.ArrowTopRight;
                }),
            Image = Image.Create(productHero.Image, cssClasses: "hero-pdp__image"),
            SecondaryLinkButton = Button.Create(productHero.SecondaryLink)
                .With(b =>
                {
                    b.Class = "hero-pdp__cta2";
                    b.Icon = SvgAliases.Icons.ArrowTopRight;
                    b.Variant = "secondary";
                }),
            NavigationLinks = productHero.HideNavigationLinks
                ? null
                : AnchorList.Create()
                    .With(a =>
                    {
                        a.CssClasses = "hero-pdp__anchor-links";
                        a.Alignment = "vertical";
                    }),
        };
    }
}
