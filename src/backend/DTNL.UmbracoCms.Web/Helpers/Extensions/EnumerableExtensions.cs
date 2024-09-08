using System.Diagnostics.CodeAnalysis;

namespace DTNL.UmbracoCms.Web.Helpers.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<T> Page<T>(
        this IEnumerable<T> items,
        int currentPage,
        int pageSize)
    {
        int skipItems = (currentPage - 1) * pageSize;

        return items.Skip(skipItems).Take(pageSize);
    }

    public static bool HasAny<T>([NotNullWhen(true)] this IEnumerable<T>? source)
    {
        return source != null && source.Any();
    }

    public static IEnumerable<T> EnsureNotNull<T>(this IEnumerable<T?>? source)
    {
        return source?.OfType<T>() ?? [];
    }

    public static bool HasAny<T>(this IEnumerable<T>? source, Func<T, bool> predicate)
    {
        return source != null && source.Any(predicate);
    }

    public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T>? source)
    {
        return source ?? [];
    }

    public static IEnumerable<string> NotNullOrWhiteSpace(
        this IEnumerable<string?>? source)
    {
        return source
            .OrEmptyIfNull()
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .Select(item => item!);
    }

    public static IEnumerable<TResult> Using<TSource, TResult>(
        this IEnumerable<TSource?>? source,
        Func<TSource, TResult?> conversionFunc)
        where TSource : class
        where TResult : class
    {
        return source
            .OrEmptyIfNull()
            .WhereNotNull()
            .Select(conversionFunc)
            .WhereNotNull();
    }

    public static IEnumerable<TResult> UsingMany<TSource, TResult>(
        this IEnumerable<TSource>? source,
        Func<TSource, IEnumerable<TResult>> conversionFunc)
        where TSource : class
        where TResult : class
    {
        return source
            .OrEmptyIfNull()
            .SelectMany(conversionFunc)
            .WhereNotNull();
    }

    public static IQueryable<T> If<T>(
        this IQueryable<T> query,
        bool should,
        params Func<IQueryable<T>, IQueryable<T>>[] transforms)
    {
        return should
            ? transforms.Aggregate(
                query,
                (current, transform) => transform.Invoke(current))
            : query;
    }

    public static IEnumerable<T> If<T>(
        this IEnumerable<T> query,
        bool should,
        params Func<IEnumerable<T>, IEnumerable<T>>[] transforms)
    {
        return should
            ? transforms.Aggregate(
                query,
                (current, transform) => transform.Invoke(current))
            : query;
    }

    /// <summary>
    /// Splits a list and returns a tuple with two lists: one with matching elements and another with the others.
    /// </summary>
    public static (List<T> MatchingElements, List<T> OtherElements) Split<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        List<T> matchingElements = [];
        List<T> otherElements = [];

        foreach (T item in source)
        {
            if (predicate(item))
            {
                matchingElements.Add(item);
            }
            else
            {
                otherElements.Add(item);
            }
        }

        return (matchingElements, otherElements);
    }

    public static IEnumerable<TResult> Distinct<TSource, TResult>(
        this IEnumerable<TSource>? source,
        Func<TSource, TResult> conversionFunc)
    {
        return source
            .OrEmptyIfNull()
            .Select(conversionFunc)
            .Distinct();
    }

    public static (T Min, T Max) MinMax<T>(this IEnumerable<T> source, T defaultMin, T defaultMax)
        where T : IComparable<T>
    {
        using IEnumerator<T> enumerator = source.GetEnumerator();

        if (!enumerator.MoveNext())
        {
            return (defaultMin, defaultMax);
        }

        T min = enumerator.Current;
        T max = enumerator.Current;

        while (enumerator.MoveNext())
        {
            T current = enumerator.Current;
            if (current.CompareTo(min) < 0)
            {
                min = current;
            }

            if (current.CompareTo(max) > 0)
            {
                max = current;
            }
        }

        return (min, max);
    }

    public static bool TryGetSingle<T>(this IEnumerable<T> source, [NotNullWhen(true)] out T? matchingElement)
    {
        matchingElement = default;

        if (source is ICollection<T> { Count: 1 })
        {
            if (source.ElementAt(0) is not { } singleElement)
            {
                return false;
            }

            matchingElement = singleElement;
            return true;

        }

        int count = 0;
        foreach (T element in source)
        {
            checked
            {
                matchingElement = element;
                count++;
            }

            if (count > 1)
            {
                break;
            }
        }

        return count == 1;
    }

    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source, int offset = 0)
    {
        return source?.Select((item, index) => (item, index + offset)) ?? [];
    }
}
