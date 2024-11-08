using System.Linq.Expressions;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace DTNL.UmbracoCms.Web.Helpers.Extensions;

public static class ContentExtensions
{
    public static void SetValue<TModel>(this IContent content, Expression<Func<TModel, object?>> property, object? value, string? culture = null)
        where TModel : IPublishedElement
    {
        content.SetValue<TModel, object?>(property, value, culture);
    }

    public static void SetValue<TModel, TValue>(this IContent content, Expression<Func<TModel, TValue>> property, object? value, string? culture = null)
        where TModel : IPublishedElement
    {
        content.SetValue(PublishedElementExtensions.GetAlias(property), value, culture);
    }

    public static TValue? GetValue<TModel, TValue>(this IContent content, Expression<Func<TModel, TValue>> property, string? culture = null)
        where TModel : IPublishedElement
    {
        return content.GetValue<TValue>(PublishedElementExtensions.GetAlias(property), culture);
    }
}
