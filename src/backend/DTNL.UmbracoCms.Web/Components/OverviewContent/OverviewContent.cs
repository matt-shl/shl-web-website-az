using DTNL.UmbracoCms.Web.Helpers;
using DTNL.UmbracoCms.Web.Helpers.Aliases;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using DTNL.UmbracoCms.Web.Models.Filters;
using DTNL.UmbracoCms.Web.Services;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class OverviewContent : OverviewFor<PageOverview, ICompositionBasePage, ContentFilters, CardKnowledge>
{
    public OverviewContent(ISearchService searchService)
        : base(searchService)
    {
    }

    public override LayoutSection LayoutSection => new()
    {
        CssClasses = "t-white",
        Variant = "grid",
        Id = "content",
        ListLabel = "Knowledge",
    };

    protected override PageOverview OverviewPage => (NodeProvider.CurrentNode as PageOverview)!;

    protected override List<ICompositionBasePage> GetPages()
    {
        return OverviewPage
            .Children<ICompositionBasePage>()
            .OrEmptyIfNull()
            .ToList();
    }

    protected override ContentFilters ApplyFilters(List<ICompositionBasePage> pages)
    {
        ContentFilters contentFilters = new(OverviewPage, Request.Query);

        contentFilters.AddFilterOptions(ContentFilters.FilterFields, pages, HttpContext);

        foreach ((string name, Func<ICompositionBasePage, IEnumerable<string>?> getValues)
                 in ContentFilters.FilterFields)
        {
            if (!contentFilters.TryGetValue(name, out FilterOption[]? filterOptions) ||
                !FilterOption.AnySelected(filterOptions))
            {
                continue;
            }

            pages.RemoveAll(page => !FilterOption.AnySelectedValueIn(filterOptions, getValues(page)));
        }

        string? sort = HttpContext.GetFilterOptions("Sort").FirstOrDefault()?.Label;
        bool hasSort = sort == CultureDictionary.GetTranslation(TranslationAliases.Common.Filters.SortOldestFirst);

        pages.Sort((x, y) => hasSort ? DateTime.Compare(y.CreateDate, x.CreateDate) : DateTime.Compare(x.CreateDate, y.CreateDate));

        return contentFilters;
    }

    protected override Filters? GetFilters(ContentFilters? filters, List<ICompositionBasePage> pages)
    {
        return filters is null ? null : Filters.Create(filters, TotalCount, pages, CultureDictionary);

    }

    protected override IEnumerable<CardKnowledge> MapToOverviewItems(List<ICompositionBasePage> pages)
    {
        return pages.Using(CardKnowledge.Create);
    }
}
