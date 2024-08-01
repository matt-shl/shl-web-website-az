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
        Title = element?.HeroTitle;

        Image? image = Image.Create(element?.Image, cssClasses: "homepage-hero__image");

        if (image == null)
        {
            return Content("");
        }

        Image = image;


        MainButton = Button.Create(element?.MainButtonLink)
            .With(b =>
            {
                b.Class = "button--icon hero-home__cta";
                b.Hook = "homepage-hero-button";
            });

        SecondaryButton = Button.Create(element?.SecondaryButtonLink)
            .With(b =>
            {
                b.Class = "button--icon hero-home__cta";
                b.Variant = "secondary ";
                b.Hook = "homepage-hero-button";
            });

        ShortDescription = element?.ShortDescription?.ToString();



        VideoUrl = Video.Create((NestedBlockVideoNativeUrl?) element?.Video?.FirstOrDefault()?.Content);


        return View("HomepageHero", this);
    }
}
