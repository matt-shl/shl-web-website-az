using DTNL.UmbracoCms.Web.Components.Hero;
using DTNL.UmbracoCms.Web.Helpers;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Core.Models;
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

    public required AnchorList AnchorLinks { get; set; }

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

            PrimaryLinkButton = Button.Create(productHero.PrimaryLink, fallBackVariant: "primary")
                .With(b => b.Class = "hero-pdp__cta1"),

            Image = Image.Create(productHero.Image, imageCropMode: ImageCropMode.Max, cssClasses: "hero-pdp__image", style: "heroPdp"),

            SecondaryLinkButton = Button.Create(productHero.SecondaryLink, fallBackVariant: "secondary")
                .With(b => b.Class = "hero-pdp__cta2"),

            AnchorLinks = AnchorList.Create(productHero),
        };
    }
}
