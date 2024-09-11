using DTNL.UmbracoCms.Web.Helpers;
using DTNL.UmbracoCms.Web.Helpers.Aliases;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using DTNL.UmbracoCms.Web.Models.Filters;
using DTNL.UmbracoCms.Web.Models.Products;
using DTNL.UmbracoCms.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Dictionary;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class OverviewProducts : ViewComponentExtended
{
    private readonly ICultureDictionary _cultureDictionary;

    public OverviewProducts(ICultureDictionary cultureDictionary)
    {
        _cultureDictionary = cultureDictionary;
    }

    public const int PageSize = 6;

    public int TotalCount { get; set; }

    public EmptySection? NoResultsSection { get; set; }

    public required Filters Filters { get; set; }

    public required List<CardProduct> ResultCards { get; set; }

    public Pagination? Pagination { get; set; }

    public LayoutSection LayoutSection
    { get; set; }
    = new()
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

        ProductFilters productFilters = GetAndApplyFilters(productPages);

        Filters = Filters.Create(productFilters, productPages, _cultureDictionary);

        ResultCards = productPages
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

    private ProductFilters GetAndApplyFilters(List<PageProduct> productPages)
    {
        ProductFilters productFilters = new()
        {
            CurrentUrl = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}").ToString(),
            OverviewUrl = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}").ToString(),
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

        string? sort = GetFilters("Sort")?.FirstOrDefault();
        bool hasSort = sort is not null && sort == _cultureDictionary.GetTranslation(TranslationAliases.Common.Filters.SortOldestFirst);

        productPages.Sort((x, y) => hasSort ? DateTime.Compare(y.CreateDate, x.CreateDate) : DateTime.Compare(x.CreateDate, y.CreateDate));

        return productFilters;
    }

    private string[]? GetFilters(string filterKey)
    {
        HttpContext.VaryByQueryKeys(filterKey);

        string? filterValue = Request.Query[filterKey];

        return filterValue?.Split(',');
    }
}
