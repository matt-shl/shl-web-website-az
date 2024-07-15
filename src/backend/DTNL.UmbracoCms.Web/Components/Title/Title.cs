using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class Title : ViewComponentExtended
{
    public required string PageTitle { get; set; }

    public IViewComponentResult Invoke(ICompositionBasePage basePage)
    {
        PageTitle = basePage.GetTitle();

        if (PageTitle is "")
        {
            return Content("");
        }

        return View("Title", this);
    }
}
