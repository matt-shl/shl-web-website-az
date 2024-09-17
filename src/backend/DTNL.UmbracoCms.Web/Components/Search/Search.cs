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

        Variant = currentNode is PageVacancyOverview ? "job" : "in-hero";

        ActionUrl = currentNode.Url();

        SearchQuery = Request.Query.GetSearchQuery();

        SearchLabel = CultureDictionary.GetTranslation(TranslationAliases.Common); // TODO

        SearchPlaceholder = CultureDictionary.GetTranslation(TranslationAliases.Common); // TODO

        if (currentNode is PageVacancyOverview vacancyOverviewPage)
        {
            VacancyFilters vacancyFilters = new(vacancyOverviewPage, Request.Query);
            List<PageVacancy> vacancyPages = Services.NodeProvider
                .GetVacancyPages(vacancyOverviewPage)
                .ToList();

            Filters = [];

            foreach ((string name, Func<PageVacancy, IEnumerable<string>?> getValues)
                     in VacancyFilters.QuickFilterFields)
            {
                Filter filter = Filter.CreateDropdownOptions(name, getValues, vacancyFilters, vacancyPages);

                Filters.Add(filter);
            }
        }

        return View("Search", this);
    }
}
