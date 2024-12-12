using DTNL.UmbracoCms.Web.Helpers.Extensions;
using DTNL.UmbracoCms.Web.Models.Filters;
using DTNL.UmbracoCms.Web.Services;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class OverviewContent : OverviewFor<PageOverview, ICompositionBasePage, BaseFilters, CardKnowledge>
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

    protected override BaseFilters? ApplyFilters(List<ICompositionBasePage> pages)
    {
        return null;
    }

    protected override Filters? GetFilters(BaseFilters? filters, List<ICompositionBasePage> pages)
    {
        return null;
    }

    protected override IEnumerable<CardKnowledge> GetOverviewItems(List<ICompositionBasePage> pages)
    {
        return pages.Using(CardKnowledge.Create);
    }
}
