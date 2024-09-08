namespace DTNL.UmbracoCms.Web.Helpers.Extensions;

public static class CollectionExtensions
{
    private static readonly Random Random = new();

    public static T? GetRandom<T>(this ICollection<T> source)
    {
        return source.ElementAtOrDefault(Random.Next(0, source.Count));
    }
}
