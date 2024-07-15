using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DTNL.UmbracoCms.Web.Helpers.TagHelpers;

/// <summary>
/// Suppresses the output of the element if the supplied value equates to <c>false</c>, white space or <c>null</c>.
/// </summary>
[HtmlTargetElement("*", Attributes = IfValueAttributeName)]
public class IfAttributeTagHelper : TagHelper
{
    private const string IfValueAttributeName = "asp-if";

    /// <summary>
    /// Gets or sets the value to test.
    /// </summary>
    [HtmlAttributeName(IfValueAttributeName)]
    public object? Value { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        bool suppressOutput = Value switch
        {
            bool boolValue => !boolValue,
            string stringValue => string.IsNullOrWhiteSpace(stringValue),
            _ => Value == null,
        };

        if (suppressOutput)
        {
            output.SuppressOutput();
        }
    }
}
