using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace DTNL.UmbracoCms.Web.Components;

public class Header : ViewComponentExtended
{
    public required Navigation Navigation { get; set; }

    public required string HomeTitle { get; set; }

    public required string OpenMenuLabel { get; set; }

    public required string MenuLabel { get; set; }

    public IViewComponentResult Invoke()
    {
        Navigation = Navigation.Create(NodeProvider.SiteSettings);

        HomeTitle = CultureDictionary.GetTranslation(Helpers.Aliases.TranslationAliases.Navigation.Home);
        OpenMenuLabel = CultureDictionary.GetTranslation(Helpers.Aliases.TranslationAliases.Navigation.OpenMenu);
        MenuLabel = CultureDictionary.GetTranslation(Helpers.Aliases.TranslationAliases.Navigation.Menu);

        return View("Header", this);
    }
}
