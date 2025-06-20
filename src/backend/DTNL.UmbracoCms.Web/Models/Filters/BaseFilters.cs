using DTNL.UmbracoCms.Web.Helpers;
using DTNL.UmbracoCms.Web.Helpers.Aliases;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Flurl;
using Umbraco.Cms.Core.Dictionary;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace DTNL.UmbracoCms.Web.Models.Filters;

public abstract class BaseFilters : Dictionary<string, FilterOption[]>
{
    protected BaseFilters(IPublishedContent overviewPage, IQueryCollection queryCollection)
    {
        CurrentUrl = overviewPage.Url(mode: UrlMode.Absolute).SetQueryParams(queryCollection);
        OverviewUrl = overviewPage.Url(mode: UrlMode.Absolute);
        SearchQuery = queryCollection.GetSearchQuery();
    }

    public FilterOption[]? Sorting { get; set; }

    public string CurrentUrl { get; private init; }

    public string OverviewUrl { get; private init; }

    public string? SearchQuery { get; private init; }

    public void AddSortingOptions(HttpContext httpContext, ICultureDictionary cultureDictionary)
    {
        Sorting = GetAllFilterOptions(
            FilterConstants.Sort,
            [
                cultureDictionary.GetTranslation(TranslationAliases.Common.Filters.SortNewestFirst),
                cultureDictionary.GetTranslation(TranslationAliases.Common.Filters.SortOldestFirst),
            ],
            httpContext);
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
        AddFilterOptions(fieldName, pages.SelectMany(p => getValues(p).OrEmptyIfNull()), httpContext);
    }

    public void AddFilterOptions(
        string fieldName,
        IEnumerable<string>? values,
        HttpContext httpContext)
    {
        TryAdd(fieldName, GetAllFilterOptions(fieldName, values, httpContext));
    }

    private static FilterOption[] GetAllFilterOptions(
        string fieldName,
        IEnumerable<string>? values,
        HttpContext httpContext)
    {
        FilterOption[] selectedFilterOptions = httpContext.GetFilterOptions(fieldName);

        FilterOption[] allFilterOptions = values
            .OrEmptyIfNull()
            .NotNullOrWhiteSpace()
            .Distinct()
            .Select(FilterOption.Create)
            .ToArray();

        foreach (FilterOption option in allFilterOptions)
        {
            option.IsSelected = selectedFilterOptions.Any(o => o.Value == option.Value);
        }

        return allFilterOptions;
    }
}
