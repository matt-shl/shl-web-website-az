using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DTNL.UmbracoCms.Web.Helpers.Extensions;

public static class HtmlHelperExtensions
{
    /// <summary>
    ///     Returns an HTML tag attribute.
    /// </summary>
    public static IHtmlContent Attribute(this IHtmlHelper? _, string? name, object? value = null)
    {
        if (name.IsNullOrEmpty())
        {
            return HtmlString.Empty;
        }

        return new TagHelperAttribute(name, value, value != null ? HtmlAttributeValueStyle.DoubleQuotes : HtmlAttributeValueStyle.Minimized);
    }

    /// <summary>
    ///     Returns a list of HTML tag attributes.
    /// </summary>
    public static IHtmlContent Attributes(this IHtmlHelper? _, IDictionary<string, string?> attributes)
    {
        HtmlContentBuilder htmlContentBuilder = new();
        foreach ((string name, string? value) in attributes)
        {
            CreateAttribute(name, value).CopyTo(htmlContentBuilder);
            htmlContentBuilder.AppendHtml(" ");
        }

        return htmlContentBuilder;
    }

    internal static TagHelperAttribute CreateAttribute(string name, string? value = null)
    {
        return new TagHelperAttribute(name, value, value != null ? HtmlAttributeValueStyle.DoubleQuotes : HtmlAttributeValueStyle.Minimized);
    }
}
