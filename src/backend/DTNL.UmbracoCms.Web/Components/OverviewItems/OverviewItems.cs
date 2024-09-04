using DTNL.UmbracoCms.Web.Helpers;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class OverviewItems : ViewComponentExtended
{
    public const int PageSize = 6;

    public required List<CardKnowledge> ResultCards { get; set; }

    public IViewComponentResult Invoke(PageKnowledgeOverview overviewPage)
    {
        HttpContext.VaryByPageNumber();

        int pageNumber = Request.Query.GetPageNumber();

        ResultCards = overviewPage
            .Children()
            .OfType<ICompositionContentDetails>()
            .Using(p => CardKnowledge.CreateOverview(p))
            .Page(pageNumber, PageSize)
            .ToList();

        return View("OverviewItems", this);
    }
}
