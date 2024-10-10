using DTNL.UmbracoCms.Web.Components.FormElements;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Flurl;
using static DTNL.UmbracoCms.Web.Components.FormElements.Checkbox;
using static DTNL.UmbracoCms.Web.Components.FormElements.SelectElement;

namespace DTNL.UmbracoCms.Web.Models.Filters;

public class Filter
{
    public required string FilterNamePrefix { get; set; }

    public required string Name { get; set; }

    public required string Type { get; set; }

    public string? Value { get; set; }

    public required List<IFormOption> Options { get; set; }

    public static Filter CreateCheckboxOptions(
        string filterName,
        string filterNamePrefix,
        BaseFilters filters,
        FilterOption? defaultOption = null)
    {
        FilterOption[]? filterOptions = filters.GetValueOrDefault(filterName);

        return new Filter
        {
            Name = filterName,
            FilterNamePrefix = filterNamePrefix,
            Type = nameof(Checkbox),
            Options = defaultOption
                .AsEnumerableOfOne()
                .EnsureNotNull()
                .Concat(filterOptions.OrEmptyIfNull())
                .Select(
                    filterOption => new CheckboxOption(
                        filterOption.Value,
                        filterOption.Label,
                        filterOption.Label,
                        description: null,
                        hook: "js-hook-filters-input",
                        attr: new Dictionary<string, string?>
                        {
                            ["data-url-replacement"] = filters.CurrentUrl.Contains(filterOption.Label)
                            ? filters
                                .CurrentUrl
                                .Replace($"{filterName}={filterOption.Label}", "")
                            : filters
                                .CurrentUrl
                                .AppendQueryParam(filterName, filterOption.Label),
                        },
                        selected: filterOption.IsSelected))
                .OfType<IFormOption>()
                .ToList(),
        };
    }

    public static Filter CreateDropdownOptions(
        string filterName,
        string filterNamePrefix,
        BaseFilters filters,
        FilterOption? defaultOption = null)
    {
        FilterOption[]? filterOptions = filters.GetValueOrDefault(filterName);

        return new Filter
        {
            Name = filterName,
            FilterNamePrefix = filterNamePrefix,
            Type = nameof(SelectElement),
            Value = filterOptions?.FirstOrDefault(option => option.IsSelected)?.Value,
            Options = defaultOption
                .AsEnumerableOfOne()
                .EnsureNotNull()
                .Concat(filterOptions.OrEmptyIfNull())
                .Select(
                    filterOption => new SelectOption(
                        filterOption.Value,
                        filterOption.Label,
                        filterOption.Label,
                        attr: new Dictionary<string, string?>
                        {
                            ["data-url-replacement"] = filters
                                .CurrentUrl
                                .AppendQueryParam(filterName, filterOption.Label),
                        }))
                .OfType<IFormOption>()
                .ToList(),
        };
    }
}
