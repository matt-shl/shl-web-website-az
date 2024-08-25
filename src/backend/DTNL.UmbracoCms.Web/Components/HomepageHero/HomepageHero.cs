using DTNL.UmbracoCms.Web.Components.Hero;
using DTNL.UmbracoCms.Web.Helpers.Aliases;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class HomepageHero : IHero
{
    public string? Title { get; set; }

    public Image? Image { get; set; }

    public Video? VideoUrl { get; set; }

    public ButtonLink? MainButton { get; set; }

    public ButtonLink? SecondaryButton { get; set; }

    public string? ShortDescription { get; set; }

    public static HomepageHero? Create(NestedBlockHomepageHero? homepageHero)
    {
        if (homepageHero is null)
        {
            return null;
        }

        return new HomepageHero()
        {
            Title = homepageHero.HeroTitle,

            Image = Image.Create(homepageHero.Image, cssClasses: "homepage-hero__image"),

            MainButton = ButtonLink.Create(homepageHero.MainButtonLink?.FirstOrDefault(), cssClasses: "button--icon hero-home__cta", jsHook: "homepage-hero-button", svgIcon: SvgAliases.Icons.ArrowTopRight),
            SecondaryButton = ButtonLink.Create(homepageHero.SecondaryButtonLink?.FirstOrDefault(), cssClasses: "button--icon hero-home__cta", jsHook: "homepage-hero-button", svgIcon: SvgAliases.Icons.ArrowTopRight),

            ShortDescription = homepageHero.ShortDescription?.ToHtmlString(),

            VideoUrl = Video.Create((NestedBlockVideoNativeUrl?) homepageHero.Video?.FirstOrDefault()?.Content, css: "c-video--background")
            .With(v =>
            {
                v.InstanceId = "hero-video";
                v.Title = "Homepage video";
                v.Description = "Homepage video description"; // TO DO: change this one once the videos are agreed and if needed for accessibility
                v.Autoplay = true;
                v.Controls = false;
                v.CustomControls = true;
                v.Muted = true;
            }),
        };
    }
}
