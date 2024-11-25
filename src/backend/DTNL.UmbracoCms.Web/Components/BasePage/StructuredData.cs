using DTNL.UmbracoCms.Web.Helpers.Extensions;
using DTNL.UmbracoCms.Web.Models.BrandfolderAssets;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components.BasePage;

public class StructuredData : ViewComponentExtended
{
    public string? CompanyName { get; set; }

    public string? HomePageUrl { get; set; }

    public string? CompanyLogo { get; set; }

    public IViewComponentResult Invoke(PageHome? homePage, SiteSettings? siteSettings)
    {
        CompanyName = siteSettings?.CompanyName;
        CompanyLogo = BrandfolderAsset
            .Create(siteSettings?.CompanyLogo)?
            .GetDefaultCropUrl(1200, 630);
        HomePageUrl = homePage?.Url();

        return View("~/Components/BasePage/StructuredData.cshtml", this);
    }
}
