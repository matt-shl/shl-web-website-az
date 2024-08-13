using Microsoft.AspNetCore.ResponseCaching;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace DTNL.UmbracoCms.Web.Helpers;

public static class QueryStringHelper
{
    public const string PageQueryString = "p";
    public const string SearchQueryString = "q";

    /// <summary>
    /// Sets the page number as a vary by query caching key.
    /// </summary>
    public static void VaryByPageNumber(this HttpContext httpContext)
    {
        httpContext.VaryByQueryKeys(PageQueryString);
    }

    /// <summary>
    /// Sets the search query as a vary by query caching key.
    /// </summary>
    public static void VaryBySearchQuery(this HttpContext httpContext)
    {
        httpContext.VaryByQueryKeys(SearchQueryString);
    }

    /// <summary>
    /// Returns the page number present in the querystring. If not present, defaults to 1.
    /// </summary>
    public static int GetPageNumber(this IQueryCollection? query)
    {
        return query?[PageQueryString] is { } queryResult && int.TryParse(queryResult, out int pageNumber)
            ? pageNumber
            : 1;
    }

    /// <summary>
    /// Returns the search query present in the querystring.
    /// </summary>
    public static string? GetSearchQuery(this IQueryCollection? query)
    {
        return query?[SearchQueryString];
    }

    /// <summary>
    /// Sets the vary by query caching keys.
    /// </summary>
    public static void VaryByQueryKeys(this HttpContext httpContext, params string[] queryKeys)
    {
        IResponseCachingFeature? responseCachingFeature = httpContext.Features.Get<IResponseCachingFeature>();

        if (responseCachingFeature == null)
        {
            return;
        }

        string[] currentVaryByQueryKeys = responseCachingFeature.VaryByQueryKeys ?? [];

        if (currentVaryByQueryKeys is ["*"])
        {
            // We already vary by any query key, nothing to do here
            return;
        }

        if (queryKeys is ["*"])
        {
            // We want to vary by all query keys, no need to merge previous entries
            responseCachingFeature.VaryByQueryKeys = queryKeys;
            return;
        }

        responseCachingFeature.VaryByQueryKeys = currentVaryByQueryKeys.Union(queryKeys).ToArray();
    }

    /// <summary>
    /// Returns the new query url based on the initial request and the given parameter.
    /// </summary>
    /// <remarks>If response caching is set, only the configured keys are used to generate the new url.</remarks>
    public static string GetNewQueryUrl(this HttpContext httpContext, string key, StringValues value)
    {
        return httpContext.GetNewQueryUrl([new KeyValuePair<string, StringValues>(key, value)]) is { } queryUrl and not ""
            ? queryUrl
            : "?";
    }

    /// <summary>
    /// Returns the new query url based on the initial request and the given parameters.
    /// </summary>
    /// <remarks>If response caching is set, only the configured keys are used to generate the new url.</remarks>
    public static string GetNewQueryUrl(this HttpContext httpContext, IEnumerable<KeyValuePair<string, StringValues>> parameters)
    {
        Dictionary<string, StringValues> query = httpContext.Request.Query.ToDictionary();
        foreach (KeyValuePair<string, StringValues> kvPair in parameters)
        {
            query[kvPair.Key] = kvPair.Value;
        }

        CleanQueryParameters(httpContext, query);

        return QueryHelpers.AddQueryString("", query);
    }

    private static void CleanQueryParameters(HttpContext httpContext, Dictionary<string, StringValues> parameters)
    {
        // Check by which query keys the url should vary
        string[]? allowedKeys = httpContext.GetVaryByQueryKeys();
        if (allowedKeys != null && !(allowedKeys.Length == 1 && allowedKeys[0] == "*"))
        {
            parameters.RemoveAll(kvPair => !allowedKeys.Contains(kvPair.Key, StringComparer.InvariantCultureIgnoreCase));
        }

        // Remove redundant first page query parameter
        if (parameters.TryGetValue(PageQueryString, out StringValues values) && values == "1")
        {
            _ = parameters.Remove(PageQueryString);
        }
    }

    /// <summary>
    /// Retrieves the vary by query caching keys.
    /// </summary>
    public static string[]? GetVaryByQueryKeys(this HttpContext httpContext)
    {
        IResponseCachingFeature? responseCachingFeature = httpContext.Features.Get<IResponseCachingFeature?>();
        if (responseCachingFeature == null)
        {
            return null;
        }

        return responseCachingFeature.VaryByQueryKeys ?? [];
    }

    private static Dictionary<string, StringValues> ToDictionary(this IQueryCollection query)
    {
        return new Dictionary<string, StringValues>(query, StringComparer.InvariantCultureIgnoreCase);
    }
}
