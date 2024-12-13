using DTNL.UmbracoCms.Web.Helpers.Extensions;
using DTNL.UmbracoCms.Web.Models.Filters;
using DTNL.UmbracoCms.Web.Services;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class OverviewSearchResults : OverviewFor<PageSearch, ICompositionBasePage, GeneralFilters, CardKnowledge>
{
    public OverviewSearchResults(ISearchService searchService)
        : base(searchService)
    {
    }

    public override LayoutSection LayoutSection => new()
    {
        Variant = "grid",
        Id = "content",
        ListLabel = "Search",
    };

    protected override PageSearch OverviewPage => NodeProvider.SearchPage!;

    protected override List<ICompositionBasePage> GetPages()
    {
        throw new NotImplementedException();
    }

    protected override GeneralFilters ApplyFilters(List<ICompositionBasePage> pages)
    {
        throw new NotImplementedException();
    }

    protected override (List<ICompositionBasePage> Pages, GeneralFilters? Filters) GetPagesAndApplyFilters()
    {
        GeneralFilters generalFilters = new(OverviewPage, Request.Query);

        generalFilters.AddFilterOptions(nameof(ICompositionContentDetails.Type), NodeProvider.SiteSettings?.Types, HttpContext);

        if (SearchTerm.IsNullOrWhiteSpace() || SearchTerm.Length < 3)
        {
            return ([], generalFilters);
        }

        string[]? selectedTypes = generalFilters[nameof(ICompositionContentDetails.Type)]
            .Where(filterOption => filterOption.IsSelected)
            .Select(filterOption => filterOption.Label)
            .ToArray();

        if (selectedTypes.Length == 0)
        {
            selectedTypes = null;
        }

        List<PublishedSearchResult> matchingResults = SearchService
            .Search(SearchTerm, new SearchFilters { PageTypes = selectedTypes }, PageNumber, PageSize, out long totalCount)
            .ToList();

        List<ICompositionBasePage> pages = matchingResults
            .Select(r => r.Content)
            .OfType<ICompositionBasePage>()
            .ToList();

        TotalCount = totalCount;

        return (pages, generalFilters);
    }

    protected override Filters? GetFilters(GeneralFilters? filters, List<ICompositionBasePage> pages)
    {
        return filters is null ? null : Filters.Create(filters, TotalCount, pages);
    }

    protected override IEnumerable<CardKnowledge> MapToOverviewItems(List<ICompositionBasePage> pages)
    {
        return pages.Using(CardKnowledge.Create);
    }

    protected override List<IOverviewItem> GetOverviewItems(List<ICompositionBasePage> pages)
    {
        return MapToOverviewItems(pages)
            .OfType<IOverviewItem>()
            .ToList();
    }
}
