using DTNL.UmbracoCms.Web.Helpers;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
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

    public void AddFilterOptions<TPage>(
        (string Name, Func<TPage, IEnumerable<string>?> GetValues)[] filterFields,
        List<TPage> pages,
        HttpContext httpContext)
    {
        foreach ((string name, Func<TPage, IEnumerable<string>?> getValues) in filterFields)
        {
            AddFilterOptions(name, getValues, pages, httpContext);
        }
    }

    public void AddFilterOptions<TPage>(
        string fieldName,
        Func<TPage, IEnumerable<string>?> getValues,
        List<TPage> pages,
        HttpContext httpContext)
    {
        FilterOption[] selectedFilterOptions = httpContext.GetFilterOptions(fieldName);

        FilterOption[] allFilterOptions = pages
            .SelectMany(p => getValues(p).OrEmptyIfNull())
            .Distinct()
            .EnsureNotNull()
            .Select(FilterOption.Create)
            .ToArray();

        foreach (FilterOption option in allFilterOptions)
        {
            option.IsSelected = selectedFilterOptions.Any(o => o.Value == option.Value);
        }

        Add(fieldName, allFilterOptions);
    }
}
