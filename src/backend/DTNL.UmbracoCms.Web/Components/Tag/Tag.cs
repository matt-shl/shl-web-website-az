namespace DTNL.UmbracoCms.Web.Components;

public class Tag
{
    public required string Label { get; set; }

    public string? CssClasses { get; set; }

    public static Tag? Create(string? label, string? cssClasses = null)
    {
        return string.IsNullOrWhiteSpace(label)
            ? null
            : new Tag
            {
                Label = label,
                CssClasses = cssClasses,
            };
    }
}
