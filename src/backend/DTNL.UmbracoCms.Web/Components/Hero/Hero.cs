using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components.Hero;

public interface IHero
{
    string ViewPath => $"~/Components/{GetType().Name}/{GetType().Name}.cshtml";
}

public class Hero : ViewComponentExtended
{
    public IViewComponentResult Invoke(ICompositionBasePage page)
    {
        IHero? hero = (page as ICompositionHero)?.Hero?.FirstOrDefault()?.Content switch
        {
            NestedBlockProductHero heroPdp => HeroPdp.Create(heroPdp),
            NestedBlockHomepageHero heroHomepage => HomepageHero.Create(heroHomepage),
            _ => null,
        };


        if (hero is null)
        {
            return Content("");
        }

        return View(hero.ViewPath, hero);
    }
}
