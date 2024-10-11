using Microsoft.AspNetCore.Mvc;

namespace DTNL.UmbracoCms.Web.Components.FormElements;

public class TextArea : ViewComponentExtended
{
    public string? Class { get; set; }

    public bool Disabled { get; set; }

    public bool LabelAsPlaceholder { get; set; }

    public required string Label { get; set; }

    public bool SrOnly { get; set; }

    public required string Id { get; set; }

    public required string Name { get; set; }

    public string? Value { get; set; }

    public string? Placeholder { get; set; }

    public int? MaxLength { get; set; }

    public string? Validate { get; set; }

    public string? RequiredError { get; set; }

    public string? Hook { get; set; }

    public string? Error { get; set; }

    public Dictionary<string, string?> Attributes { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public IViewComponentResult Invoke(
        string name,
        string label,
        string id,
        string? classes = default,
        bool disabled = default,
        string? value = default,
        string? placeholder = default,
        int? maxLength = default,
        string? validate = default,
        string? requiredError = default,
        string? hook = default,
        string? error = default,
        bool srOnly = default,
        Dictionary<string, string?>? attr = default,
        bool labelAsPlaceholder = default)
    {
        Name = name;
        Label = label;
        Id = id;
        Class = classes;
        Disabled = disabled;
        Value = value;
        Placeholder = placeholder;
        Validate = validate;
        Hook = hook;
        Error = error;
        RequiredError = requiredError;
        MaxLength = maxLength;
        SrOnly = srOnly;
        LabelAsPlaceholder = labelAsPlaceholder;
        Attributes = attr ?? [];
        return View("~/Components/FormElements/TextArea.cshtml", this);
    }
}
