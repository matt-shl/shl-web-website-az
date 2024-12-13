using DTNL.UmbracoCms.Web.Components.FormElements;
using DTNL.UmbracoCms.Web.Helpers.Aliases;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using DTNL.UmbracoCms.Web.Models.Filters;
using DTNL.UmbracoCms.Web.Models.Products;
using DTNL.UmbracoCms.Web.Models.Vacancies;
using Umbraco.Cms.Core.Dictionary;
using Umbraco.Cms.Web.Common.PublishedModels;
using static DTNL.UmbracoCms.Web.Components.FormElements.Radio;

namespace DTNL.UmbracoCms.Web.Components;

public class FiltersModal
{
    public int ResultsCount { get; set; }

    public required string ResultsOverviewPageUrl { get; set; }

    public required List<Filter> Filters { get; set; }

    public Sort? Sorter { get; set; }

    public string? SearchQuery { get; set; }

    public static FiltersModal Create(
        ProductFilters productFilters,
        List<PageProduct> productPages,
        ICultureDictionary cultureDictionary)
    {
        List<Filter> filters = [];

        foreach (string filterName in productFilters.Keys)
        {
            filters.Add(Filter.CreateCheckboxOptions(filterName, TranslationAliases.Products, productFilters));
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

        foreach (string filterName in vacancyFilters.Keys)
        {
            filters.Add(Filter.CreateCheckboxOptions(filterName, TranslationAliases.Vacancies, vacancyFilters));
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

    public static FiltersModal Create(
        GeneralFilters generalFilters,
        List<ICompositionBasePage> pages)
    {
        List<Filter> filters = [];

        foreach (string filterName in generalFilters.Keys)
        {
            filters.Add(Filter.CreateCheckboxOptions(filterName, TranslationAliases.Search, generalFilters));
        }

        return new FiltersModal
        {
            ResultsCount = pages.Count,
            ResultsOverviewPageUrl = generalFilters.OverviewUrl,
            SearchQuery = generalFilters.SearchQuery,
            Filters = filters,
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
                        isChecked: !hasSort
                    ),
                    new RadioOption(
                        "filter-sorting-oldest",
                        cultureDictionary.GetTranslation(TranslationAliases.Common.Filters.SortOldestFirst),
                        cultureDictionary.GetTranslation(TranslationAliases.Common.Filters.SortOldestFirst),
                        hook: "js-hook-filters-input",
                        isChecked: hasSort),
                    ],
            };
        }
    }
}
