using DTNL.UmbracoCms.Web.Components.PartialComponent;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components.Hero;

public interface IHero : IPartialViewPath;

public class Hero : ViewComponentExtended
{
    public IViewComponentResult Invoke(ICompositionBasePage page)
    {
        IHero? hero = (page as ICompositionHero)?.Hero?.FirstOrDefault()?.Content switch
        {
            NestedBlockProductHero heroPdp => HeroPdp.Create(heroPdp, page),
            NestedBlockHomepageHero heroHomepage => HomepageHero.Create(heroHomepage, page),
            _ => null,
        };


        if (hero is null)
        {
            return Content("");
        }

        return View(hero.ViewPath, hero);
    }
}
