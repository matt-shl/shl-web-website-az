using DTNL.UmbracoCms.Web.Helpers.TagHelpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;

namespace DTNL.UmbracoCms.Web.Helpers.Extensions;

public static class RazorPageExtensions
{
    /// <summary>
    ///     In a Razor partial view, renders the child content defined in the parent view.
    /// </summary>
    public static IHtmlContent? RenderPartialBody<TModel>(this RazorPage<TModel> razorPage)
    {
        return razorPage.ViewData[PartialTagHelper.ViewDataPartialBody] as IHtmlContent;
    }
}
