using DTNL.UmbracoCms.Web.Components.FormElements;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using DTNL.UmbracoCms.Web.Models.Filters;
using DTNL.UmbracoCms.Web.Models.Products;
using Umbraco.Cms.Web.Common.PublishedModels;
using static DTNL.UmbracoCms.Web.Components.FormElements.Checkbox;

namespace DTNL.UmbracoCms.Web.Components;

public class FiltersModal
{
    public int ResultsCount { get; set; }

    public required string ResultsOverviewPageUrl { get; set; }

    public required List<Filter> Filters { get; set; }

    public static FiltersModal Create(
        ProductFilters productFilters,
        PageProductOverview productOverviewPage,
        List<PageProduct> productPages)
    {
        List<Filter> filters = [];

        foreach ((string name, Func<PageProduct, IEnumerable<string>?> getValues)
                 in ProductFilters.FilterFields)
        {
            filters.Add(Filter.Create(name, getValues, productFilters, productPages));
        }

        return new FiltersModal
        {
            ResultsCount = productPages.Count,
            ResultsOverviewPageUrl = productOverviewPage.Url(),
            Filters = filters,
        };
    }

    public class Filter
    {
        public required string Name { get; set; }

        public required string Type { get; set; }

        public required List<IFormOption> Options { get; set; }

        public static Filter Create(
            string name,
            Func<PageProduct, IEnumerable<string>?> getValues,
            ProductFilters productFilters,
            List<PageProduct> productPages)
        {
            return new Filter
            {
                Name = name,
                Type = nameof(Checkbox),
                Options = productPages
                    .SelectMany(p => getValues(p).OrEmptyIfNull())
                    .Distinct()
                    .Select(FilterOption.CreateForSearch)
                    .Select(filterOption => new CheckboxOption(
                        filterOption.Id,
                        filterOption.Title,
                        filterOption.Id,
                        selected: productFilters.IsSelected(name, filterOption)))
                    .OfType<IFormOption>()
                    .ToList(),
            };
        }
    }
}
