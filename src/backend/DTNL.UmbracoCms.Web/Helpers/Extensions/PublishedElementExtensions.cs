using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Infrastructure.ModelsBuilder;

namespace DTNL.UmbracoCms.Web.Helpers.Extensions;

public static class PublishedElementExtensions
{
    public static string GetViewComponentName(this IPublishedElement publishedElement)
    {
        return publishedElement.ContentType.Alias.ToFirstUpper(CultureInfo.InvariantCulture);
    }

    public static string GetAlias<TModel, TValue>(this TModel? _, Expression<Func<TModel, TValue>> property)
        where TModel : IPublishedElement
    {
        return GetAlias(property);
    }

    public static string GetAlias<TModel, TValue>(Expression<Func<TModel, TValue>> property)
        where TModel : IPublishedElement
    {
        MemberInfo? member;

        if (property is LambdaExpression { Body: MemberExpression { Expression.NodeType: ExpressionType.Parameter } memberExpression1 })
        {
            member = memberExpression1.Member;
        }
        else if (property is LambdaExpression { Body: UnaryExpression { NodeType: ExpressionType.Convert or ExpressionType.ConvertChecked, Operand: MemberExpression { Expression.NodeType: ExpressionType.Parameter } memberExpression2 } })
        {
            member = memberExpression2.Member;
        }
        else
        {
            throw new ArgumentException("Not a proper lambda expression.", nameof(property));
        }

        if (member.GetCustomAttribute<ImplementPropertyTypeAttribute>() is not { } attribute)
        {
            throw new InvalidOperationException("Property is not marked with ImplementPropertyType attribute.");
        }

        return attribute.Alias;
    }

}
