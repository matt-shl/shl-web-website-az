using DTNL.UmbracoCms.Web.Components.FormElements;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Flurl;
using Umbraco.Cms.Core.Models.PublishedContent;
using static DTNL.UmbracoCms.Web.Components.FormElements.Checkbox;
using static DTNL.UmbracoCms.Web.Components.FormElements.SelectElement;

namespace DTNL.UmbracoCms.Web.Models.Filters;

public class Filter
{
    public required string Name { get; set; }

    public required string Type { get; set; }

    public string? Value { get; set; }

    public required List<IFormOption> Options { get; set; }

    public static Filter CreateCheckboxOptions<TPage>(
        string name,
        Func<TPage, IEnumerable<string>?> getValues,
        BaseFilters filters,
        List<TPage> pages,
        string? defaultOption = null)
        where TPage : class, IPublishedContent
    {
        return new Filter
        {
            Name = name,
            Type = nameof(Checkbox),
            Options = defaultOption
                .AsEnumerableOfOne()
                .Concat(pages.SelectMany(p => getValues(p).OrEmptyIfNull()))
                .Distinct()
                .EnsureNotNull()
                .Select(FilterOption.CreateForSearch)
                .Select(
                    filterOption => new CheckboxOption(
                        filterOption.Id,
                        filterOption.Title,
                        filterOption.Title,
                        description: null,
                        hook: "js-hook-filters-input",
                        attr: new Dictionary<string, string?>
                        {
                            ["data-url-replacement"] = filters
                                .CurrentUrl
                                .AppendQueryParam(name, filterOption.Title),
                        },
                        selected: filters.IsSelected(name, filterOption)))
                .OfType<IFormOption>()
                .ToList(),
        };
    }

    public static Filter CreateDropdownOptions<TPage>(
        string name,
        Func<TPage, IEnumerable<string>?> getValues,
        BaseFilters filters,
        List<TPage> pages,
        string? defaultOption = null)
        where TPage : class, IPublishedContent
    {
        return new Filter
        {
            Name = name,
            Type = nameof(SelectElement),
            Options = defaultOption
                .AsEnumerableOfOne()
                .Concat(pages.SelectMany(p => getValues(p).OrEmptyIfNull()))
                .Distinct()
                .EnsureNotNull()
                .Select(FilterOption.CreateForSearch)
                .Select(
                    filterOption => new SelectOption(
                        filterOption.Id,
                        filterOption.Title,
                        filterOption.Title,
                        attr: new Dictionary<string, string?>
                        {
                            ["data-url-replacement"] = filters
                                .CurrentUrl
                                .AppendQueryParam(name, filterOption.Title),
                        }))
                .OfType<IFormOption>()
                .ToList(),
        };
    }
}
