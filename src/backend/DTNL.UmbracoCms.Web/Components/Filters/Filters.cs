using DTNL.UmbracoCms.Web.Helpers.Aliases;
using DTNL.UmbracoCms.Web.Models.Filters;
using DTNL.UmbracoCms.Web.Models.Products;
using DTNL.UmbracoCms.Web.Models.Vacancies;
using Umbraco.Cms.Core.Dictionary;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class Filters
{
    public int ResultsCount { get; set; }

    public required string FilterNamePrefix { get; set; }

    public required FiltersModal FiltersModal { get; set; }

    public static Filters Create(
        ProductFilters productFilters,
        List<PageProduct> productPages,
        ICultureDictionary cultureDictionary)
    {
        return new Filters
        {
            ResultsCount = productPages.Count,
            FilterNamePrefix = TranslationAliases.Products,
            FiltersModal = FiltersModal.Create(productFilters, productPages, cultureDictionary),
        };
    }

    public static Filters Create(
        VacancyFilters vacancyFilters,
        List<PageVacancy> vacancyPages,
        ICultureDictionary cultureDictionary)
    {
        return new Filters
        {
            ResultsCount = vacancyPages.Count,
            FilterNamePrefix = TranslationAliases.Vacancies,
            FiltersModal = FiltersModal.Create(vacancyFilters, vacancyPages, cultureDictionary),
        };
    }

    public static Filters Create(
        GeneralFilters generalFilters,
        List<ICompositionBasePage> pages)
    {
        return new Filters
        {
            ResultsCount = pages.Count,
            FilterNamePrefix = TranslationAliases.Search,
            FiltersModal = FiltersModal.Create(generalFilters, pages),
        };
    }
}
