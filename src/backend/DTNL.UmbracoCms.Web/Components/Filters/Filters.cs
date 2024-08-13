using DTNL.UmbracoCms.Web.Models.Products;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class Filters
{
    public int ResultsCount { get; set; }

    public required FiltersModal FiltersModal { get; set; }

    public static Filters Create(
        ProductFilters productFilters,
        List<PageProduct> productPages)
    {
        return new Filters
        {
            ResultsCount = productPages.Count,
            FiltersModal = FiltersModal.Create(productFilters, productPages),
        };
    }
}
