using DTNL.UmbracoCms.Web.Helpers.Extensions;

namespace DTNL.UmbracoCms.Web.Models.Filters;

public record FilterOption
{
    public required string Title { get; set; }

    public required string Id { get; set; }

    public bool Selected { get; set; }

    public static FilterOption CreateForSearch(string term)
    {
        return new FilterOption
        {
            Id = term.ToUrlString(),
            Title = term,
            Selected = true,
        };
    }
}
