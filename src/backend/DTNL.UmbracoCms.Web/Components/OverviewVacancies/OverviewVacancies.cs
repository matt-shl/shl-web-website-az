using DTNL.UmbracoCms.Web.Helpers;
using DTNL.UmbracoCms.Web.Helpers.Aliases;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using DTNL.UmbracoCms.Web.Models.Filters;
using DTNL.UmbracoCms.Web.Models.Vacancies;
using DTNL.UmbracoCms.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class OverviewVacancies : ViewComponentExtended
{
    public const int PageSize = 6;

    private readonly ISearchService _searchService;

    public OverviewVacancies(ISearchService searchService)
    {
        _searchService = searchService;
    }

    public int TotalCount { get; set; }

    public EmptySection? NoResultsSection { get; set; }

    public required Filters Filters { get; set; }

    public required List<JobListingItem> Results { get; set; }

    public Pagination? Pagination { get; set; }

    public LayoutSection LayoutSection { get; set; } = new()
    {
        CssClasses = "t-white",
        Variant = "grid-single-column",
        ReduceMargin = "top-bottom",
        Id = "content",
    };

    public IViewComponentResult Invoke(PageVacancyOverview vacancyOverviewPage)
    {
        LayoutSection.ListLabel = CultureDictionary.GetTranslation(TranslationAliases.Vacancies);

        HttpContext.VaryByPageNumber();
        HttpContext.VaryBySearchQuery();
        int pageNumber = Request.Query.GetPageNumber();
        string? searchQuery = Request.Query.GetSearchQuery();

        List<PageVacancy> vacancyPages = vacancyOverviewPage
            .Children<PageVacancy>()
            .OrEmptyIfNull()
            .ToList();

        VacancyFilters vacancyFilters = GetAndApplyFilters(vacancyOverviewPage, vacancyPages);

        if (!searchQuery.IsNullOrWhiteSpace())
        {
            List<PublishedSearchResult> matchingResults = _searchService
                .Search(searchQuery, vacancyPages.Select(p => p.Id), 1, int.MaxValue, out _)
                .ToList();

            vacancyPages.RemoveAll(p =>
                matchingResults.All(r => r.Content.Id != p.Id));
        }

        Filters = Filters.Create(vacancyFilters, vacancyPages, CultureDictionary);

        Results = vacancyPages
            .Using(JobListingItem.Create)
            .Page(pageNumber, PageSize)
            .ToList();

        TotalCount = vacancyPages.Count;

        if (TotalCount == 0)
        {
            NoResultsSection = EmptySection.Create(vacancyOverviewPage);
        }

        Pagination = Pagination.Create(pageNumber, TotalCount, PageSize);

        return View("OverviewVacancies", this);
    }

    private VacancyFilters GetAndApplyFilters(
        PageVacancyOverview vacancyOverviewPage,
        List<PageVacancy> vacancyPages)
    {
        VacancyFilters vacancyFilters = new(vacancyOverviewPage, Request.Query);

        foreach ((string name, Func<PageVacancy, IEnumerable<string>?> getValues)
                 in VacancyFilters.FilterFields)
        {
            if (GetFilters(name) is not { Length: > 0 } filters)
            {
                continue;
            }

            vacancyPages.RemoveAll(p => !getValues(p)
                .OrEmptyIfNull()
                .ContainsAny(filters));

            vacancyFilters.Add(
                name,
                filters.Select(FilterOption.CreateForSearch).ToArray());
        }

        string? sort = GetFilters("Sort")?.FirstOrDefault();
        bool hasSort = sort == CultureDictionary.GetTranslation(TranslationAliases.Common.Filters.SortOldestFirst);

        vacancyPages.Sort((x, y) => hasSort ? DateTime.Compare(y.CreateDate, x.CreateDate) : DateTime.Compare(x.CreateDate, y.CreateDate));

        return vacancyFilters;
    }

    private string[]? GetFilters(string filterKey)
    {
        HttpContext.VaryByQueryKeys(filterKey);

        string? filterValue = Request.Query[filterKey];

        return filterValue?.Split(',');
    }
}
