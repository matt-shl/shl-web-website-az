using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DTNL.UmbracoCms.Web.Helpers.TagHelpers;

/// <summary>
/// Adds conditional rendering support for data attributes, when the value is <c>null</c>.
/// </summary>
[HtmlTargetElement("*", Attributes = $"{ConditionalDataAttributePrefix}-*")]
public class ConditionalDataAttributeTagHelper : TagHelper
{
    private const string ConditionalDataAttributePrefix = "asp-data";

    /// <summary>
    /// Gets or sets the values to test.
    /// </summary>
    [HtmlAttributeName(ConditionalDataAttributePrefix)]
    public IDictionary<string, object?> Values { get; set; } = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        foreach ((string key, object? value) in Values)
        {
            if (value == null)
            {
                // Null valued attributes are not rendered
                continue;
            }

            // Find the original asp-data attribute index
            int originalIndex = context.AllAttributes.IndexOfName($"{ConditionalDataAttributePrefix}-{key}");
            if (originalIndex == -1)
            {
                continue;
            }

            // Try to find the index where we should output our data attribute
            // (Immediately before any of the following original attributes, or if none of them are found, at the end)
            int outputIndex = output.Attributes.Count;
            foreach (TagHelperAttribute nextAttribute in context.AllAttributes.Skip(originalIndex + 1))
            {
                int nextAttributeOutputIndex = output.Attributes.IndexOf(nextAttribute);
                if (nextAttributeOutputIndex != -1)
                {
                    outputIndex = nextAttributeOutputIndex;
                    break;
                }
            }

            TagHelperAttribute originalAttribute = context.AllAttributes[originalIndex];
            output.Attributes.Insert(outputIndex, new TagHelperAttribute($"data-{key}", originalAttribute.Value, originalAttribute.ValueStyle));
        }
    }
}
