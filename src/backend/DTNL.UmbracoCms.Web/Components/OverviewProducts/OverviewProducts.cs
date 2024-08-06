using DTNL.UmbracoCms.Web.Helpers;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using DTNL.UmbracoCms.Web.Models.Filters;
using DTNL.UmbracoCms.Web.Models.Products;
using DTNL.UmbracoCms.Web.Services;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class OverviewProducts : ViewComponentExtended
{
    public const int PageSize = 6;

    public int TotalCount { get; set; }

    public required Filters Filters { get; set; }

    public required List<CardProduct> ResultCards { get; set; }

    public Pagination? Pagination { get; set; }

    public IViewComponentResult Invoke(PageProductOverview productOverviewPage)
    {
        HttpContext.VaryByPageNumber();
        int pageNumber = Request.Query.GetPageNumber();

        List<PageProduct> productPages = NodeProvider
            .GetProductPages(productOverviewPage)
            .ToList();

        ProductFilters productFilters = GetAndApplyFilters(productPages);

        Filters = Filters.Create(productFilters, productPages);

        ResultCards = productPages
            .Using(p => CardProduct.Create(p))
            .Page(pageNumber, PageSize)
            .ToList();

        TotalCount = productPages.Count;

        Pagination = Pagination.Create(pageNumber, TotalCount, PageSize);

        return View("OverviewProducts", this);
    }

    private ProductFilters GetAndApplyFilters(List<PageProduct> productPages)
    {
        ProductFilters productFilters = new()
        {
            CurrentUrl = Request.GetEncodedPathAndQuery(),
        };

        foreach ((string name, Func<PageProduct, IEnumerable<string>?> getValues)
                 in ProductFilters.FilterFields)
        {
            if (GetFilters(name) is not { Length: > 0 } filters)
            {
                continue;
            }

            productPages.RemoveAll(p => !getValues(p)
                .OrEmptyIfNull()
                .ContainsAny(filters));

            productFilters.Add(
                name,
                filters.Select(FilterOption.CreateForSearch).ToArray());
        }

        return productFilters;
    }

    private string[]? GetFilters(string filterKey)
    {
        HttpContext.VaryByQueryKeys(filterKey);

        string? filterValue = Request.Query[filterKey];

        return filterValue?.Split(',');
    }
}
