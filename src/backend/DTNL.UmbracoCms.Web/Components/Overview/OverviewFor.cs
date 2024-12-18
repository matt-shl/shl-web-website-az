using DTNL.UmbracoCms.Web.Components.PartialComponent;
using DTNL.UmbracoCms.Web.Helpers;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using DTNL.UmbracoCms.Web.Models.Filters;
using DTNL.UmbracoCms.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

#pragma warning disable SA1402 // Suppress warning File may only contain a single type

public abstract class OverviewFor<TOverviewPage, TPage, TFilters, TOverviewItem> : Overview
    where TOverviewPage : ICompositionBasePage, ICompositionNoResults
    where TPage : ICompositionBasePage
    where TFilters : BaseFilters
    where TOverviewItem : IOverviewItem
{
    protected const int PageSize = 12;

    protected OverviewFor(ISearchService searchService)
    {
        SearchService = searchService;
    }

    protected bool ShowNoResultsSection { get; set; } = true;

    protected ISearchService SearchService { get; }

    protected abstract TOverviewPage OverviewPage { get; }

    protected abstract List<TPage> GetPages();

    protected abstract TFilters? ApplyFilters(List<TPage> pages);

    protected virtual (List<TPage> Pages, TFilters? Filters) GetPagesAndApplyFilters()
    {
        List<TPage> pages = GetPages();

        TFilters? filters = ApplyFilters(pages);

        string? searchQuery = Request.Query.GetSearchQuery();

        if (!searchQuery.IsNullOrWhiteSpace())
        {
            List<PublishedSearchResult> matchingResults = SearchService
                .Search(searchQuery, new SearchFilters { Ids = pages.Select(p => p.Id) }, 1, pages.Count, out _)
                .ToList();

            pages.RemoveAll(p =>
                matchingResults.All(r => r.Content.Id != p.Id));
        }

        TotalCount = pages.Count;

        return (pages, filters);
    }

    protected abstract Filters? GetFilters(TFilters? filters, List<TPage> pages);

    protected abstract IEnumerable<TOverviewItem> GetOverviewItems(List<TPage> pages);

    public IViewComponentResult Invoke()
    {
        HttpContext.VaryByPageNumber();
        HttpContext.VaryBySearchQuery();

        PageNumber = Request.Query.GetPageNumber();

        (List<TPage> pages, TFilters? filters) = GetPagesAndApplyFilters();

        Filters = GetFilters(filters, pages);

        Items = GetOverviewItems(pages)
            .OfType<IOverviewItem>()
            .Page(PageNumber, PageSize)
            .ToList();

        if (OverviewPage is PageVacancyOverview)
        {
            IsJobsOverview = true;
        }

        OverviewType = pages.FirstOrDefault() switch
        {
            PagePublication => "publication",
            PageEvent => "event",
            PageNews => "news",
            PageProduct => "product",
            _ => null,
        };

        SearchTerm = Request.Query.GetSearchQuery();

        if (TotalCount == 0 && ShowNoResultsSection)
        {
            NoResultsSection = EmptySection.Create(OverviewPage);
        }

        Pagination = Pagination.Create(PageNumber, TotalCount, PageSize);

        return View("~/Components/Overview/Overview.cshtml", this);
    }
}

public interface IOverviewItem : IPartialViewPath;

public abstract class Overview : ViewComponentExtended
{
    public int PageNumber { get; set; }

    public long TotalCount { get; set; }

    public EmptySection? NoResultsSection { get; set; }

    public Filters? Filters { get; set; }

    public required List<IOverviewItem> Items { get; set; }

    public Pagination? Pagination { get; set; }

    public abstract LayoutSection LayoutSection { get; }

    public string? OverviewType { get; set; }

    public string? SearchTerm { get; set; }

    public bool IsJobsOverview { get; set; }
}

public interface JobListItem
{
    public string Url { get; set; }

    public string Title { get; set; }

    public string Location { get; set; }

    public List<string> Tags { get; set; }
}
