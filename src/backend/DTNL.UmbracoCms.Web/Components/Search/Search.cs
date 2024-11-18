using DTNL.UmbracoCms.Web.Helpers;
using DTNL.UmbracoCms.Web.Helpers.Aliases;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using DTNL.UmbracoCms.Web.Models.Filters;
using DTNL.UmbracoCms.Web.Models.Vacancies;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class Search : ViewComponentExtended
{
    public required string Variant { get; set; }

    public required string ActionUrl { get; set; }

    public string? SearchQuery { get; set; }

    public required string SearchLabel { get; set; }

    public required string SearchPlaceholder { get; set; }

    public List<Filter>? Filters { get; set; }

    public string? CssClasses { get; set; }

    public IViewComponentResult Invoke(bool isFlyoutSearch = false)
    {
        if (GetSearchResultsPage(isFlyoutSearch, out bool isVacancySearch) is not { } searchResultsPage)
        {
            return Content("");
        }

        if (!isFlyoutSearch)
        {
            Variant = isVacancySearch ? "job" : "in-hero";
        }

        ActionUrl = searchResultsPage.Url();

        SearchQuery = Request.Query.GetSearchQuery();

        if (isVacancySearch)
        {
            SearchLabel = CultureDictionary.GetTranslation(TranslationAliases.Vacancies.SearchLabel);

            SearchPlaceholder = CultureDictionary.GetTranslation(TranslationAliases.Vacancies.SearchPlaceholder);

            SetVacancyFilters(NodeProvider.VacancyOverviewPage!);
        }
        else
        {
            SearchLabel = CultureDictionary.GetTranslation(TranslationAliases.Search.SearchLabel);

            SearchPlaceholder = CultureDictionary.GetTranslation(TranslationAliases.Search.SearchPlaceholder);
        }

        return View("Search", this);
    }

    private ICompositionBasePage? GetSearchResultsPage(bool isFlyoutSearch, out bool isVacancySearch)
    {
        isVacancySearch = !isFlyoutSearch && NodeProvider.CurrentNode is PageVacancyOverview or PageCareerOverview;

        return isVacancySearch ? NodeProvider.VacancyOverviewPage : NodeProvider.SearchPage;
    }

    private void SetVacancyFilters(PageVacancyOverview vacancyOverviewPage)
    {
        List<PageVacancy> vacancyPages = Services.NodeProvider
            .GetVacancyPages(vacancyOverviewPage)
            .ToList();

        VacancyFilters vacancyFilters = new(vacancyOverviewPage, Request.Query);

        vacancyFilters.AddFilterOptions(VacancyFilters.QuickFilterFields, vacancyPages, HttpContext);

        Filters = [];

        foreach (string filterName in vacancyFilters.Keys)
        {
            FilterOption defaultOption = new()
            {
                Label = CultureDictionary.GetTranslation($"{TranslationAliases.Vacancies.AllFilterOptions}.{filterName}"),
                Value = string.Empty,
            };

            Filter filter = Filter
                .CreateDropdownOptions(
                    filterName,
                    TranslationAliases.Vacancies,
                    vacancyFilters,
                    defaultOption);

            Filters.Add(filter);
        }
    }
}
