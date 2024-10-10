using DTNL.UmbracoCms.Web.Components.Hero;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class HomepageHero : IHero
{
    public required string Title { get; set; }

    public Image? Image { get; set; }

    public Video? VideoUrl { get; set; }

    public Button? MainButton { get; set; }

    public Button? SecondaryButton { get; set; }

    public string? ShortDescription { get; set; }

    public static HomepageHero? Create(NestedBlockHomepageHero? homepageHero)
    {
        if (homepageHero is null)
        {
            return null;
        }

        return new HomepageHero
        {
            Title = homepageHero.Title!,
            Image = Image.Create(homepageHero.Image, cssClasses: "homepage-hero__image"),
            MainButton = Button
                .Create(homepageHero.MainButtonLink, fallBackVariant: "primary")
                .With(b =>
                {
                    b.Class = "button--icon hero-home__cta";
                    b.Hook = "homepage-hero-button";
                }),
            SecondaryButton = Button
                .Create(homepageHero.SecondaryButtonLink, fallBackVariant: "secondary")
                .With(b =>
                {
                    b.Class = "button--icon hero-home__cta";
                    b.Hook = "homepage-hero-button";
                }),
            ShortDescription = homepageHero.Text?.ToHtmlString(),

            VideoUrl = Video.Create((NestedBlockVideoNativeUrl?) homepageHero.Video?.FirstOrDefault()?.Content, css: "c-video--background")
            .With(v =>
            {
                v.InstanceId = "hero-video";
                v.Title = "Homepage video";
                v.Description = "Homepage video description"; // TODO: change this one once the videos are agreed and if needed for accessibility
                v.Autoplay = true;
                v.Controls = false;
                v.CustomControls = true;
                v.Muted = true;
            }),
        };
    }
}
