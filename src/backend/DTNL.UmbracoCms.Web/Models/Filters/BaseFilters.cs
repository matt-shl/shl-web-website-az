using Flurl;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace DTNL.UmbracoCms.Web.Models.Filters;

public abstract class BaseFilters : Dictionary<string, FilterOption[]>
{
    public string CurrentUrl { get; init; }

    public string OverviewUrl { get; init; }

    protected BaseFilters(IPublishedContent overviewPage, IQueryCollection queryCollection)
    {
        CurrentUrl = overviewPage.Url(mode: UrlMode.Absolute).SetQueryParams(queryCollection);
        OverviewUrl = overviewPage.Url(mode: UrlMode.Absolute);
    }

    public bool IsSelected(string name, FilterOption option)
    {
        return TryGetValue(name, out FilterOption[]? filterOptions) && filterOptions.Contains(option);
    }
}
