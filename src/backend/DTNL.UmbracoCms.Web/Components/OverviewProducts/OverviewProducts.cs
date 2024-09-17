using DTNL.UmbracoCms.Web.Helpers;
using DTNL.UmbracoCms.Web.Helpers.Aliases;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using DTNL.UmbracoCms.Web.Models.Filters;
using DTNL.UmbracoCms.Web.Models.Products;
using DTNL.UmbracoCms.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class OverviewProducts : ViewComponentExtended
{
    public const int PageSize = 6;

    public int TotalCount { get; set; }

    public EmptySection? NoResultsSection { get; set; }

    public required Filters Filters { get; set; }

    public required List<CardProduct> Results { get; set; }

    public Pagination? Pagination { get; set; }

    public LayoutSection LayoutSection { get; set; } = new()
    {
        CssClasses = "t-white",
        Variant = "grid",
        Id = "content",
    };

    public IViewComponentResult Invoke(PageProductOverview productOverviewPage)
    {
        HttpContext.VaryByPageNumber();
        int pageNumber = Request.Query.GetPageNumber();

        List<PageProduct> productPages = NodeProvider
            .GetProductPages(productOverviewPage)
            .ToList();

        ProductFilters productFilters = GetAndApplyFilters(productOverviewPage, productPages);

        Filters = Filters.Create(productFilters, productPages, CultureDictionary);

        Results = productPages
            .Using(p => CardProduct.Create(p))
            .Page(pageNumber, PageSize)
            .ToList();

        TotalCount = productPages.Count;

        if (TotalCount == 0)
        {
            NoResultsSection = EmptySection.Create(productOverviewPage);
        }

        Pagination = Pagination.Create(pageNumber, TotalCount, PageSize);

        return View("OverviewProducts", this);
    }

    private ProductFilters GetAndApplyFilters(
        PageProductOverview productOverviewPage,
        List<PageProduct> productPages)
    {
        ProductFilters productBaseFilters = new(productOverviewPage, Request.Query);

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

            productBaseFilters.Add(
                name,
                filters.Select(FilterOption.CreateForSearch).ToArray());
        }

        string? sort = GetFilters("Sort")?.FirstOrDefault();
        bool hasSort = sort == CultureDictionary.GetTranslation(TranslationAliases.Common.Filters.SortOldestFirst);

        productPages.Sort((x, y) => hasSort ? DateTime.Compare(y.CreateDate, x.CreateDate) : DateTime.Compare(x.CreateDate, y.CreateDate));

        return productBaseFilters;
    }

    private string[]? GetFilters(string filterKey)
    {
        HttpContext.VaryByQueryKeys(filterKey);

        string? filterValue = Request.Query[filterKey];

        return filterValue?.Split(',');
    }
}
