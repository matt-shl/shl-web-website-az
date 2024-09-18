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

        ProductFilters selectedProductFilters = GetAndApplySelectedFilters(productOverviewPage, productPages);

        Filters = Filters.Create(selectedProductFilters, productPages, CultureDictionary);

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

    private ProductFilters GetAndApplySelectedFilters(
        PageProductOverview productOverviewPage,
        List<PageProduct> productPages)
    {
        ProductFilters productFilters = new(productOverviewPage, Request.Query);

        productFilters.AddFilterOptions(ProductFilters.FilterFields, productPages, HttpContext);

        foreach ((string name, Func<PageProduct, IEnumerable<string>?> getValues)
                 in ProductFilters.FilterFields)
        {
            if (!productFilters.TryGetValue(name, out FilterOption[]? filterOptions) ||
                !FilterOption.AnySelected(filterOptions))
            {
                continue;
            }

            productPages.RemoveAll(page => !FilterOption.AnySelectedValueIn(filterOptions, getValues(page)));
        }

        string? sort = HttpContext.GetFilterOptions("Sort").FirstOrDefault()?.Label;
        bool hasSort = sort == CultureDictionary.GetTranslation(TranslationAliases.Common.Filters.SortOldestFirst);

        productPages.Sort((x, y) => hasSort ? DateTime.Compare(y.CreateDate, x.CreateDate) : DateTime.Compare(x.CreateDate, y.CreateDate));

        return productFilters;
    }
}
