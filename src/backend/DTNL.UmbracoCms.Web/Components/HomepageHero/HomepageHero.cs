using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class HomepageHero : ViewComponentExtended
{
    public string? Title { get; set; }

    public Image? Image { get; set; }

    public Video? VideoUrl { get; set; }

    public Button? MainButton { get; set; }

    public Button? SecondaryButton { get; set; }

    public string? ShortDescription { get; set; }

    public IViewComponentResult Invoke(PageHome? element)
    {
        if (element?.Hero?.FirstOrDefault()?.Content is not NestedBlockHomepageHero hero)
        {
            return Content("");
        }

        Title = hero.HeroTitle;

        Image? image = Image.Create(hero.Image, cssClasses: "homepage-hero__image");

        if (image == null)
        {
            return Content("");
        }

        Image = image;

        MainButton = Button.Create(hero.MainButtonLink)
            .With(b =>
            {
                b.Class = "button--icon hero-home__cta";
                b.Hook = "homepage-hero-button";
            });

        SecondaryButton = Button.Create(hero.SecondaryButtonLink)
            .With(b =>
            {
                b.Class = "button--icon hero-home__cta";
                b.Variant = "secondary ";
                b.Hook = "homepage-hero-button";
            });

        ShortDescription = hero?.ShortDescription?.ToString();



        VideoUrl = Video.Create((NestedBlockVideoNativeUrl?) hero?.Video?.FirstOrDefault()?.Content);


        return View("HomepageHero", this);
    }
}
