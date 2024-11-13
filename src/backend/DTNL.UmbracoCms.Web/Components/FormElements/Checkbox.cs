using Microsoft.AspNetCore.Mvc;

namespace DTNL.UmbracoCms.Web.Components.FormElements;

public class Checkbox : ViewComponentExtended
{
    public string? Class { get; set; }

    public string? Type { get; set; }

    public required string Name { get; set; }

    public Dictionary<string, string?> Attributes { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public bool Horizontal { get; set; }

    public bool BottomMargin { get; set; }

    public bool AlignRight { get; set; }

    public bool Disabled { get; set; }

    public bool Required { get; set; }

    public string? RequiredError { get; set; }

    public string? Error { get; set; }

    public bool Sronly { get; set; }

    public bool LabelAsPlaceholder { get; set; }

    public required IEnumerable<CheckboxOption> Options { get; set; }

    public IViewComponentResult Invoke(
        string name,
        IEnumerable<CheckboxOption> options,
        string? classes = default,
        string? type = default,
        Dictionary<string, string?>? attr = default,
        bool horizontal = false,
        bool disabled = default,
        bool required = default,
        string? error = default,
        bool sronly = default,
        bool labelAsPlaceholder = default)
    {
        Name = name;
        Options = options;
        Class = classes;
        Type = type;
        Attributes = attr ?? Attributes;
        Horizontal = horizontal;
        Disabled = disabled;
        Required = required;
        Error = error;
        Sronly = sronly;
        LabelAsPlaceholder = labelAsPlaceholder;

        return View("~/Components/FormElements/Checkbox.cshtml", this);
    }

    public class CheckboxOption : IFormOption
    {
        public CheckboxOption(
            string? id,
            string? label,
            string? value,
            string? description = null,
            string? hook = null,
            Dictionary<string, string?>? attr = default,
            string? validate = null,
            bool selected = default)
        {
            Id = id;
            Label = label;
            Value = value;
            Description = description;
            Hook = hook;
            Attributes = attr ?? Attributes;
            Validate = validate;
            Selected = selected;
        }

        public string? Label { get; set; }

        public string? Id { get; set; }

        public string? Value { get; set; }

        public string? Description { get; set; }

        public string? Name { get; set; }

        public string? Hook { get; set; }

        public Dictionary<string, string?> Attributes { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        public string? Validate { get; set; }

        public bool Selected { get; set; }
    }
}
