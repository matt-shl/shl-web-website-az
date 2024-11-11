using DTNL.UmbracoCms.Web.Helpers.Extensions;

namespace DTNL.UmbracoCms.Web.Models.Filters;

public record FilterOption
{
    public required string Label { get; set; }

    public required string Value { get; set; }

    public bool IsSelected { get; set; }

    public static FilterOption Create(string term, bool isSelected)
    {
        return new FilterOption
        {
            Value = term.ToUrlString(),
            Label = term,
            IsSelected = isSelected,
        };
    }

    public static FilterOption Create(string term)
    {
        return Create(term, isSelected: false);
    }

    public static bool AnySelected(FilterOption[] filterOptions)
    {
        return filterOptions.Any(filterOption => filterOption.IsSelected);
    }

    public static bool AnySelectedValueIn(FilterOption[] filterOptions, IEnumerable<string>? values)
    {
        values ??= values.OrEmptyIfNull().ToArray();

        return filterOptions.Any(filterOption => filterOption.IsSelected && values.Contains(filterOption.Label));
    }
}
