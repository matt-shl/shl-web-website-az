using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Models.Filters;

public class ContentFilters : BaseFilters
{
    public static readonly (string Name, Func<ICompositionBasePage, IEnumerable<string>?> GetValues)[] FilterFields =
    [
        (nameof(CompositionContentDetails.ContentTags), p => p.Value<IEnumerable<string>>(nameof(CompositionContentDetails.ContentTags).ToFirstLowerInvariant())),
    ];

    public ContentFilters(PageOverview searchPage, IQueryCollection queryCollection)
        : base(searchPage, queryCollection)
    {
    }
}
