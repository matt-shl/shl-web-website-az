using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Umbraco.Cms.Core.Constants.HttpContext;

namespace DTNL.UmbracoCms.Web.Components;

public class Header : ViewComponentExtended
{
    //public Header()
    //{
    //}

    public NestedBlockNavigation? Navigation { get; set; }

    public Website? CurrentWebsite { get; set; }

    //public List<Website> Websites { get; set; } = [];

    public IViewComponentResult Invoke(IPublishedContent currentPage)
    {
        SiteSettings? siteSettings = NodeProvider.SiteSettings;
        NestedBlockNavigation? navigation = GetNavigation(currentPage, siteSettings);

        if (navigation is not null)
        {
            Navigation = navigation;
           

           

            //if (Navigation.MainPage is { } mainPage)
            //{
            //    Website website = new() { Link = Link.Create(mainPage, showTitle: false), Current = true };
            //    CurrentWebsite = website;
            //    Websites.Add(website);
            //}

            //NestedBlockNavigation? otherNavigation = navigations.Value.OtherNavigation;
            //if (otherNavigation?.MainPage is { } otherMainPage)
            //{
            //    //Websites.Add(new Website { Link = Link.Create(otherMainPage, showTitle: false), Current = false });
            //}
        }

        return View("Header", this);
    }

    /// <summary>
    /// Gets the main and other navigations based on the current node and its ancestors.
    /// </summary>
    private NestedBlockNavigation? GetNavigation(
        IPublishedContent currentPage, SiteSettings? settings)
    {
       return settings!.MainHeader!.Content;

        // return currentPage.Get(settings, Request.Path) switch
        // {
        //    WebsiteSection.Get => (settings!.MainHeader!.Content, settings.MainHeader.Content),
        //    _ => null,
        // };
    }

    public class Website
    {
        public required Link Link { get; set; }

        public bool Current { get; set; }
    }
}
