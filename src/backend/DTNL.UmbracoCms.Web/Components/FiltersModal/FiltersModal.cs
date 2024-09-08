using DTNL.UmbracoCms.Web.Components.FormElements;
using DTNL.UmbracoCms.Web.Helpers.Aliases;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using DTNL.UmbracoCms.Web.Models.Products;
using Flurl;
using Umbraco.Cms.Core.Dictionary;
using Umbraco.Cms.Web.Common.PublishedModels;
using static DTNL.UmbracoCms.Web.Components.FormElements.Checkbox;
using static DTNL.UmbracoCms.Web.Components.FormElements.Radio;
using FilterOption = DTNL.UmbracoCms.Web.Models.Filters.FilterOption;

namespace DTNL.UmbracoCms.Web.Components;

public class FiltersModal
{
    public int ResultsCount { get; set; }

    public required string ResultsOverviewPageUrl { get; set; }

    public required string OverviewPageUrl { get; set; }

    public required List<Filter> Filters { get; set; }

    public required Sort Sorter { get; set; }

    public static FiltersModal Create(
        ProductFilters productFilters,
        List<PageProduct> productPages,
        ICultureDictionary cultureDictionary)
    {
        List<Filter> filters = [];

        Sort sort = Sort.Create("Sort", productFilters, cultureDictionary);

        foreach ((string name, Func<PageProduct, IEnumerable<string>?> getValues)
                 in ProductFilters.FilterFields)
        {
            filters.Add(Filter.Create(name, getValues, productFilters, productPages,));
        }

        return new FiltersModal
        {
            ResultsCount = productPages.Count,
            ResultsOverviewPageUrl = productFilters.CurrentUrl.SetQueryParams(null),
            OverviewPageUrl = productFilters.OverviewUrl,
            Filters = filters,
            Sorter = sort,
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
            List<PageProduct> productPages
            )
        {
            return new Filter
            {
                Name = name,
                Type = nameof(Checkbox),
                Options = productPages
                    .SelectMany(p => getValues(p).OrEmptyIfNull())
                    .Distinct()
                    .Select(FilterOption.CreateForSearch)
                    .Select(
                        filterOption => new CheckboxOption(
                            filterOption.Id,
                            filterOption.Title,
                            filterOption.Title,
                            null,
                            hook: "js-hook-filters-input",
                            attr: new Dictionary<string, string?>
                            {
                                ["data-url-replacement"] = productFilters
                                    .CurrentUrl
                                    .AppendQueryParam(name, filterOption.Title),
                            },
                            selected: productFilters.IsSelected(name, filterOption)))
                    .ToList(),
            };
        }
    }

    public class Sort
    {
        public required string Name { get; set; }

        public required string Type { get; set; }

        public required List<RadioOption> Options { get; set; }

        public static Sort Create(
            string name,
            ProductFilters productFilters,
            ICultureDictionary cultureDictionary
        )
        {
            bool hasSort = productFilters.CurrentUrl.Contains("Sort");

            return new Sort
            {
                Name = name,
                Type = nameof(Radio),
                Options = [
                    new RadioOption(
                        "filter-sorting-newest",
                        cultureDictionary.GetTranslation(TranslationAliases.Common.Filters.SortNewestFirst),
                        cultureDictionary.GetTranslation(TranslationAliases.Common.Filters.SortNewestFirst),
                        hook: "js-hook-filters-input",
                        attr: new Dictionary<string, string?>
                        {
                            ["data-url-replacement"] = productFilters
                            .OverviewUrl,
                        },
                        null,
                        !hasSort
                    ),
                    new RadioOption(
                        "filter-sorting-oldest",
                        cultureDictionary.GetTranslation(TranslationAliases.Common.Filters.SortOldestFirst),
                        cultureDictionary.GetTranslation(TranslationAliases.Common.Filters.SortOldestFirst),
                        hook: "js-hook-filters-input",
                        attr: new Dictionary<string, string?>
                        {
                            ["data-url-replacement"] = productFilters
                            .CurrentUrl
                                .AppendQueryParam(name, cultureDictionary.GetTranslation(TranslationAliases.Common.Filters.SortOldestFirst)),
                        },
                        null,
                        hasSort
                    )
                    ],
            };
        }
    }
}
