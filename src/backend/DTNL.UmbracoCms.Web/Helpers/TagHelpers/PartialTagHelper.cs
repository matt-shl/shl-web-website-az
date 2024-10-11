using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Buffers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DTNL.UmbracoCms.Web.Helpers.TagHelpers;

/// <summary>
/// Renders a partial view with support for child content.
/// </summary>
[HtmlTargetElement("partial", Attributes = "name", TagStructure = TagStructure.NormalOrSelfClosing)]
public class PartialTagHelper : Microsoft.AspNetCore.Mvc.TagHelpers.PartialTagHelper
{
    public const string ViewDataPartialBody = "PartialBody";

    public PartialTagHelper(ICompositeViewEngine viewEngine, IViewBufferScope viewBufferScope)
        : base(viewEngine, viewBufferScope)
    {
    }

    /// <summary>
    /// HTML wrapper element
    /// </summary>
    public string? WrapperElement { get; set; }

    /// <inheritdoc />
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        object? originalData = ViewContext.ViewData[ViewDataPartialBody];

        try
        {
            ViewContext.ViewData[ViewDataPartialBody] = await output.GetChildContentAsync();

            await base.ProcessAsync(context, output);

            if (!WrapperElement.IsNullOrWhiteSpace())
            {
                TagBuilder wrappedContent = new(WrapperElement);

                wrappedContent.InnerHtml.AppendHtml(output.Content.GetContent());

                output.Content.SetHtmlContent(wrappedContent);
            }
        }
        finally
        {
            if (originalData != null)
            {
                ViewContext.ViewData[ViewDataPartialBody] = originalData;
            }
            else
            {
                _ = ViewContext.ViewData.Remove(ViewDataPartialBody);
            }
        }
    }
}
