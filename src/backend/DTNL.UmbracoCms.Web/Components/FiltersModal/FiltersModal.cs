using DTNL.UmbracoCms.Web.Components.FormElements;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using DTNL.UmbracoCms.Web.Models.Filters;
using DTNL.UmbracoCms.Web.Models.Products;
using Flurl;
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
            ResultsOverviewPageUrl = productFilters.CurrentUrl.SetQueryParams(null),
            Filters = filters,
        };
    }

    public class Filter
    {
        public required string Name { get; set; }

        public required string Type { get; set; }

        public required List<CheckboxOption> Options { get; set; }

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
                        attr: new Dictionary<string, string?>
                        {
                            ["data-url-replacement"] = productFilters
                                .CurrentUrl
                                .AppendQueryParam(name, filterOption.Id),
                        },
                        selected: productFilters.IsSelected(name, filterOption)))
                    .ToList(),
            };
        }
    }
}
