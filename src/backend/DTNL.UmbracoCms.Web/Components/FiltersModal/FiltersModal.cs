using DTNL.UmbracoCms.Web.Components.FormElements;
using DTNL.UmbracoCms.Web.Helpers.Aliases;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using DTNL.UmbracoCms.Web.Models.Filters;
using DTNL.UmbracoCms.Web.Models.Products;
using DTNL.UmbracoCms.Web.Models.Vacancies;
using Flurl;
using Umbraco.Cms.Core.Dictionary;
using Umbraco.Cms.Web.Common.PublishedModels;
using static DTNL.UmbracoCms.Web.Components.FormElements.Radio;

namespace DTNL.UmbracoCms.Web.Components;

public class FiltersModal
{
    public int ResultsCount { get; set; }

    public required string ResultsOverviewPageUrl { get; set; }

    public required List<Filter> Filters { get; set; }

    public required Sort Sorter { get; set; }

    public static FiltersModal Create(
        ProductFilters productFilters,
        List<PageProduct> productPages,
        ICultureDictionary cultureDictionary)
    {
        List<Filter> filters = [];

        foreach ((string name, Func<PageProduct, IEnumerable<string>?> getValues)
                 in ProductFilters.FilterFields)
        {
            filters.Add(Filter.CreateCheckboxOptions(name, getValues, productFilters, productPages));
        }

        Sort sort = Sort.Create(cultureDictionary.GetTranslation(TranslationAliases.Common.Filters.Sort), productFilters, cultureDictionary);

        return new FiltersModal
        {
            ResultsCount = productPages.Count,
            ResultsOverviewPageUrl = productFilters.OverviewUrl,
            Filters = filters,
            Sorter = sort,
        };
    }

    public static FiltersModal Create(
        VacancyFilters vacancyFilters,
        List<PageVacancy> vacancyPages,
        ICultureDictionary cultureDictionary)
    {
        List<Filter> filters = [];

        foreach ((string name, Func<PageVacancy, IEnumerable<string>?> getValues)
                 in VacancyFilters.FilterFields)
        {
            filters.Add(Filter.CreateCheckboxOptions(name, getValues, vacancyFilters, vacancyPages));
        }

        Sort sort = Sort
            .Create(cultureDictionary.GetTranslation(TranslationAliases.Common.Filters.Sort), vacancyFilters, cultureDictionary);

        return new FiltersModal
        {
            ResultsCount = vacancyPages.Count,
            ResultsOverviewPageUrl = vacancyFilters.OverviewUrl,
            Filters = filters,
            Sorter = sort,
        };
    }

    public class Sort
    {
        public required string Name { get; set; }

        public required string Type { get; set; }

        public required List<RadioOption> Options { get; set; }

        public static Sort Create(
            string name,
            BaseFilters filters,
            ICultureDictionary cultureDictionary)
        {
            bool hasSort = filters.CurrentUrl.Contains(cultureDictionary.GetTranslation(TranslationAliases.Common.Filters.Sort));

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
                            ["data-url-replacement"] = filters.OverviewUrl,
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
                            ["data-url-replacement"] = filters.CurrentUrl
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
