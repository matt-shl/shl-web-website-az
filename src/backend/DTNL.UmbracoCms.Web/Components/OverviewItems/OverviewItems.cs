using DTNL.UmbracoCms.Web.Helpers;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class OverviewItems : ViewComponentExtended
{
    public const int PageSize = 6;

    public int TotalCount { get; set; }

    public EmptySection? NoResultsSection { get; set; }

    public required Filters Filters { get; set; }

    public required List<CardKnowledge> ResultCards { get; set; }

    public Pagination? Pagination { get; set; }

    public LayoutSection LayoutSection
    { get; set; }
    = new()
    {
        CssClasses = $"t-white",
        Variant = "grid",
        Id = "content",
    };

    public IViewComponentResult Invoke(PageOverview overviewPage)
    {
        HttpContext.VaryByPageNumber();

        int pageNumber = Request.Query.GetPageNumber();

        ResultCards = overviewPage
            .Children()
            .OfType<ICompositionCardDetails>()
            .Using(p => CardKnowledge.CreateOverview(p))
            .Page(pageNumber, PageSize)
            .ToList();

        TotalCount = ResultCards.Count;

        if (TotalCount == 0)
        {
            NoResultsSection = EmptySection.Create(overviewPage);
            LayoutSection.CssClasses = "t-white c-empty-section";
            LayoutSection.ReduceMargin = "bottom";
        }

        Pagination = Pagination.Create(pageNumber, TotalCount, PageSize);

        return View("OverviewItems", this);
    }
}
