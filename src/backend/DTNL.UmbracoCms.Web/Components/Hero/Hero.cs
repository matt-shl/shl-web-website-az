using DTNL.UmbracoCms.Web.Components.PartialComponent;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components.Hero;

public interface IHero : IPartialViewPath;

public class Hero : ViewComponentExtended
{
    public IViewComponentResult Invoke(IPublishedContent page)
    {
        if (page.ContentType.Alias is "pageHome")
        {
            IHero? hero = HomepageHero.Create((page as ICompositionHomePage)?.Hero?.FirstOrDefault()?.Content as NestedBlockHomepageHero);

            if (hero is null)
            {
                return Content("");
            }

            return View(hero.ViewPath, hero);
        }
        else
        {
            IHero? hero = (page as ICompositionHero)?.Hero?.FirstOrDefault()?.Content switch
            {
                NestedBlockProductHero heroPdp => HeroPdp.Create(heroPdp, (ICompositionContentBlocks) page),
                NestedBlockHomepageHero heroHomepage => HomepageHero.Create(heroHomepage),
                NestedBlockContentHero heroContent => HeroContent.Create(heroContent, (ICompositionBasePage) page),
                _ => null,
            };
            if (hero is null)
            {
                return Content("");
            }

            return View(hero.ViewPath, hero);
        }
    }
}
