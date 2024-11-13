namespace DTNL.UmbracoCms.Web.Components.FormElements;

public record class Input
{
    public string? Class { get; set; }

    public string? Name { get; set; }

    public string? Id { get; set; }

    public string? Label { get; set; }

    public string? Type { get; set; }

    public string? Size { get; set; }

    public bool Required { get; set; }

    public string? RequiredError { get; set; }

    public bool Disabled { get; set; }

    public bool LabelAsPlaceholder { get; set; }

    public string? Placeholder { get; set; }

    public bool SrOnly { get; set; }

    public string? Value { get; set; }

    public string? Hook { get; set; }

    public string? Pattern { get; set; }

    public string? Validate { get; set; }

    public int? MaxLength { get; set; }

    public Dictionary<string, string?> Attributes { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public string? Error { get; set; }
}
