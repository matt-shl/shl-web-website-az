using DTNL.UmbracoCms.Web.Helpers.Aliases;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class Header : ViewComponentExtended
{
    public NestedBlockNavigation? Navigation { get; set; }

    public string? HomeTitle { get; set; }

    public string? OpenMenuLabel { get; set; }

    public IViewComponentResult Invoke()
    {
        SiteSettings? siteSettings = NodeProvider.SiteSettings;
        NestedBlockNavigation? navigation = GetNavigation(siteSettings);

        if (navigation is not null)
        {
            Navigation = navigation;
            HomeTitle = CultureDictionary.GetTranslation(TranslationAliases.Navigation.Home);
            OpenMenuLabel = CultureDictionary.GetTranslation(TranslationAliases.Navigation.Openmenu);
        }

        return View("Header", this);
    }

    private NestedBlockNavigation? GetNavigation(SiteSettings? settings)
    {
       return settings!.MainHeader!.Content;
    }
}
