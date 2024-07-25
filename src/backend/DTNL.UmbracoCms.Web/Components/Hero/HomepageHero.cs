using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class HomepageHero : ViewComponentExtended
{
    public string? Title { get; set; }

    public Image? Image { get; set; }

    public string? VideoUrl { get; set; }

    public Button? MainButton { get; set; }

    public Button? SecondaryButton { get; set; }

    public string? ShortDescription { get; set; }

    public IViewComponentResult Invoke(NestedBlockHero? element)
    {
        if (element == null)
        {
            return View("/Components/Hero/HomepageHero.cshtml", this);
        }

        Image? image = Image.Create(element.HeroImage);

        if (image == null)
        {
            return View("/Components/Hero/HomepageHero.cshtml", this);
        }

        image.Classes = "homepage-hero__image";

        Title = element.HeroTitle;

        MainButton = element.MainButtonLink is null ? null : Button.Create(element.MainButtonLink)
            .With(b =>
            {
                b.Class = "button--icon hero-home__cta";
                b.Hook = "homepage-hero-button";
            });

        SecondaryButton = element.MainButtonLink is null ? null : Button.Create(element.SecondaryButtonLink)
            .With(b =>
            {
                b.Class = "button--icon hero-home__cta";
                b.Variant = "secondary ";
                b.Hook = "homepage-hero-button";
            });

        Image = image;
        ShortDescription = element.ShortDescription;
        VideoUrl = element.HeroVideo;

        return View("/Components/Hero/HomepageHero.cshtml", this);
    }
}
