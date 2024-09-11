using DTNL.UmbracoCms.Web.Components.PartialComponent;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components.Hero;

public interface IHero : IPartialViewPath;

public class Hero : ViewComponentExtended
{
    public IViewComponentResult Invoke(IPublishedContent page)
    {
        IHero? hero = (page as IPageHero)?.Hero?.FirstOrDefault()?.Content switch
        {
            NestedBlockProductHero heroPdp => HeroPdp.Create(heroPdp, (ICompositionContentBlocks) page),
            NestedBlockHomepageHero heroHomepage => HomepageHero.Create(heroHomepage),
            NestedBlockContentHero heroContent => HeroContent.Create(heroContent, (ICompositionBasePage) page),
            NestedBlockSloganHero heroSlogan => Slogan.Create(heroSlogan),
            _ => null,
        };

        if (hero is null)
        {
            return Content("");
        }

        return View(hero.ViewPath, hero);
    }
}
