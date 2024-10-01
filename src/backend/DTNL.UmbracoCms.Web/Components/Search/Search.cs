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

    public IViewComponentResult Invoke()
    {
        if (NodeProvider.CurrentNode is not { } currentNode)
        {
            return Content("");
        }

        Variant = currentNode is PageVacancyOverview or PageCareerOverview ? "job" : "in-hero";

        ActionUrl = currentNode.Url();

        SearchQuery = Request.Query.GetSearchQuery();

        SearchLabel = CultureDictionary.GetTranslation(TranslationAliases.Vacancies.Search);

        SearchPlaceholder = CultureDictionary.GetTranslation(TranslationAliases.Vacancies.SearchPlaceholder);

        if (currentNode is PageVacancyOverview or PageCareerOverview &&
            NodeProvider.VacancyOverviewPage is { } vacancyOverviewPage)
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

        return View("Search", this);
    }
}
