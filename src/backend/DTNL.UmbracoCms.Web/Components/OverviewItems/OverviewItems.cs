using DTNL.UmbracoCms.Web.Helpers;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using DTNL.UmbracoCms.Web.Models.Filters;
using DTNL.UmbracoCms.Web.Models.Products;
using DTNL.UmbracoCms.Web.Services;
using Microsoft.AspNetCore.Http.Extensions;
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

        List<ICompositionKnowledgePage> knowledgePages = NodeProvider
            .GetOverviewPages(overviewPage)
            .ToList();

        ResultCards = knowledgePages
            .Using(p => CardKnowledge.CreateOverview(p))
            .Page(pageNumber, PageSize)
            .ToList();

        return View("OverviewItems", this);
    }
}
