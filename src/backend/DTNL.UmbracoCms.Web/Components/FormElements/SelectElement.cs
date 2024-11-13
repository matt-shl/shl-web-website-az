using Microsoft.AspNetCore.Mvc;

namespace DTNL.UmbracoCms.Web.Components.FormElements;

public class SelectElement : ViewComponentExtended
{
    public string? Class { get; set; }

    public required string Label { get; set; }

    public string? Value { get; set; }

    public required string Id { get; set; }

    public required string Name { get; set; }

    public Dictionary<string, string?>? Attributes { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public bool Disabled { get; set; }

    public bool Required { get; set; }

    public string? RequiredError { get; set; }

    public string? Validate { get; set; }

    public string? Error { get; set; }

    public bool SrOnly { get; set; }

    public string? Hook { get; set; }

    public bool LabelAsPlaceholder { get; set; }

    public required IEnumerable<SelectOption>? Options { get; set; }

    public required IEnumerable<SelectOptGroup>? OptGroups { get; set; }

    public IViewComponentResult Invoke(
        string id,
        string name,
        string label,
        IEnumerable<SelectOption>? options = null,
        IEnumerable<SelectOptGroup>? optGroups = null,
        string? classes = default,
        Dictionary<string, string?>? attr = default,
        bool disabled = default,
        bool required = default,
        string? requiredError = default,
        string? validate = default,
        string? value = default,
        string? error = default,
        string? hook = default,
        bool sronly = default,
        bool labelAsPlaceholder = default)
    {
        Id = id;
        Name = name;
        Label = label;
        Options = options;
        OptGroups = optGroups;
        Class = classes;
        Attributes = attr ?? Attributes;
        Disabled = disabled;
        Required = required;
        Validate = validate;
        Value = value;
        Error = error;
        RequiredError = requiredError;
        Hook = hook.IfNullOrWhiteSpace(null);
        SrOnly = sronly;
        LabelAsPlaceholder = labelAsPlaceholder;

        return View("~/Components/FormElements/SelectElement.cshtml", this);
    }

    public class SelectOptGroup(
        string? label,
        IEnumerable<SelectOption> options)
    {
        public string? Label { get; set; } = label;

        public IEnumerable<SelectOption> Options { get; set; } = options;
    }

    public class SelectOption(
        string value,
        string? id = null,
        string? label = null,
        string? lang = null,
        bool disabled = false,
        bool hidden = false,
        bool isDefault = false,
        Dictionary<string, string?>? attr = default,
        string? validate = null) : IFormOption
    {
        public string? Label { get; set; } = label ?? value;

        public string? Id { get; set; } = id;

        public string Value { get; set; } = value;

        public string? Lang { get; set; } = lang;

        public bool Disabled { get; set; } = disabled;

        public bool Hidden { get; set; } = hidden;

        public bool IsDefault { get; set; } = isDefault;

        public Dictionary<string, string?>? Attributes { get; set; } = attr;

        public string? Validate { get; set; } = validate;
    }
}
