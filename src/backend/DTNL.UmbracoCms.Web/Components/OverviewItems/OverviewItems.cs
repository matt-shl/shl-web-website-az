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

    public int TotalCount { get; set; }

    public EmptySection? NoResultsSection { get; set; }

    public required Filters Filters { get; set; }

    public required List<CardKnowledge> ResultCards { get; set; }

    public Pagination? Pagination { get; set; }

    public IViewComponentResult Invoke(PageKnowledgeOverview overviewPage)
    {
        HttpContext.VaryByPageNumber();
        int pageNumber = Request.Query.GetPageNumber();

        List<ICompositionKnowledgePage> knowledgePages = NodeProvider
            .GetOverviewPages(overviewPage)
            .ToList();

        //ProductFilters knowledgeFilters = GetAndApplyFilters(knowledgePages);

        //Filters = Filters.Create(knowledgeFilters, knowledgePages);

        ResultCards = knowledgePages
            .Using(p => CardKnowledge.CreateOverview(p))
            .Page(pageNumber, PageSize)
            .ToList();

        //TotalCount = knowledgePages.Count;

        //if (TotalCount == 0)
        //{
        //    NoResultsSection = EmptySection.Create(knowledgeOverviewPage);
        //}

        //Pagination = Pagination.Create(pageNumber, TotalCount, PageSize);

        return View("OverviewItems", this);
    }

    private ProductFilters GetAndApplyFilters(List<NestedBlockPageCard> productPages)
    {
        ProductFilters productFilters = new()
        {
            CurrentUrl = Request.GetEncodedPathAndQuery(),
        };

        //foreach ((string name, Func<PageProduct, IEnumerable<string>?> getValues)
        //         in ProductFilters.FilterFields)
        //{
        //    if (GetFilters(name) is not { Length: > 0 } filters)
        //    {
        //        continue;
        //    }

        //    productPages.RemoveAll(p => !getValues(p)
        //        .OrEmptyIfNull()
        //        .ContainsAny(filters));

        //    productFilters.Add(
        //        name,
        //        filters.Select(FilterOption.CreateForSearch).ToArray());
        //}

        return productFilters;
    }

    private string[]? GetFilters(string filterKey)
    {
        HttpContext.VaryByQueryKeys(filterKey);

        string? filterValue = Request.Query[filterKey];

        return filterValue?.Split(',');
    }
}
