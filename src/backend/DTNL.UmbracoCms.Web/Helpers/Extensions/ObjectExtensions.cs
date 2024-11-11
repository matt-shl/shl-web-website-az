using System.Diagnostics.CodeAnalysis;

namespace DTNL.UmbracoCms.Web.Helpers.Extensions;

public static class ObjectExtensions
{
    [return: NotNullIfNotNull(nameof(source))]
    public static T? With<T>(this T? source, Action<T> action)
    {
        if (source is not null)
        {
            action(source);
        }

        return source;
    }
}
