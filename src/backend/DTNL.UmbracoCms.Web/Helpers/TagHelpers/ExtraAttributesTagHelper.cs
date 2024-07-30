using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DTNL.UmbracoCms.Web.Helpers.TagHelpers;

/// <summary>
/// Adds the additional supplied attributes to the element being rendered.
/// </summary>
[HtmlTargetElement("*", Attributes = ExtraAttributesAttributeName)]
public class ExtraAttributesTagHelper : TagHelper
{
    private const string ExtraAttributesAttributeName = "asp-extra-attribs";

    /// <summary>
    /// Gets or sets the additional attributes.
    /// </summary>
    [HtmlAttributeName(ExtraAttributesAttributeName)]
    public IDictionary<string, string?> ExtraAttributes { get; set; } = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc />
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        foreach ((string name, string? value) in ExtraAttributes)
        {
            output.Attributes.SetAttribute(HtmlHelperExtensions.CreateAttribute(name, value));
        }
    }
}
