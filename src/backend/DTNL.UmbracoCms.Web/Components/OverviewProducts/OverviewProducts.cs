using DTNL.UmbracoCms.Web.Helpers.Aliases;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using DTNL.UmbracoCms.Web.Models.Filters;
using DTNL.UmbracoCms.Web.Models.Products;
using DTNL.UmbracoCms.Web.Services;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class OverviewProducts : OverviewFor<PageProductOverview, PageProduct, ProductFilters, CardProduct>
{
    public OverviewProducts(ISearchService searchService)
        : base(searchService)
    {
    }

    public override LayoutSection LayoutSection => new()
    {
        CssClasses = "t-white",
        Variant = "grid",
        Id = "content",
        ListLabel = CultureDictionary.GetTranslation(TranslationAliases.Products.ProductsList),
    };

    protected override PageProductOverview OverviewPage => (NodeProvider.CurrentNode as PageProductOverview)!;

    protected override List<PageProduct> GetPages()
    {
        return NodeProvider
            .GetProductPages(OverviewPage)
            .ToList();
    }

    protected override ProductFilters ApplyFilters(List<PageProduct> pages)
    {
        ProductFilters productFilters = new(OverviewPage, Request.Query);

        productFilters.AddFilterOptions(ProductFilters.FilterFields, pages, HttpContext);

        foreach ((string name, Func<PageProduct, IEnumerable<string>?> getValues)
                 in ProductFilters.FilterFields)
        {
            if (!productFilters.TryGetValue(name, out FilterOption[]? filterOptions) ||
                !FilterOption.AnySelected(filterOptions))
            {
                continue;
            }

            pages.RemoveAll(page => !FilterOption.AnySelectedValueIn(filterOptions, getValues(page)));
        }

        GetAndApplySorting(productFilters, pages);

        return productFilters;
    }

    protected override Filters? GetFilters(ProductFilters? filters, List<PageProduct> pages)
    {
        return filters is null ? null : Filters.Create(filters, TotalCount, pages, CultureDictionary);
    }

    protected override IEnumerable<CardProduct> MapToOverviewItems(List<PageProduct> pages)
    {
        return pages.Using(p => CardProduct.Create(p).With(card => card.ShowSpecs = false));
    }
}
