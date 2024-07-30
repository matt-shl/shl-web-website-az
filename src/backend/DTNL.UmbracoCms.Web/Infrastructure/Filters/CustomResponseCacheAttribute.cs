using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.ResponseCaching;
using Microsoft.Net.Http.Headers;

namespace DTNL.UmbracoCms.Web.Infrastructure.Filters;

/// <summary>
/// Specifies the parameters necessary for setting appropriate headers in response caching.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class CustomResponseCacheAttribute : Attribute, IActionFilter, IOrderedFilter
{
    // A nullable-int cannot be used as an Attribute parameter.
    // Hence this nullable fields to back the properties.
    private int? _serverDuration;
    private int? _duration;
    private bool? _noStore;

    /// <summary>
    /// Gets or sets the duration in seconds for which the response is cached.
    /// This sets "max-age" in "Cache-control" header.
    /// </summary>
    public int Duration
    {
        get => _duration ?? 0;
        set => _duration = value;
    }

    /// <summary>
    /// Gets or sets the duration in seconds for which the response is cached in the shared caches (Server/CDN).
    /// </summary>
    /// <remarks>When this value isn't set the <see cref="Duration"/> property is used instead.</remarks>
    public int ServerDuration
    {
        get => _serverDuration ?? 0;
        set => _serverDuration = value;
    }

    /// <summary>
    /// Gets or sets the location where the data from a particular URL must be cached.
    /// </summary>
    public ResponseCacheLocation Location { get; set; } = ResponseCacheLocation.Any;

    /// <summary>
    /// Gets or sets a value indicating whether the data should be stored or not.
    /// When set to <see langword="true"/>, it sets "Cache-control" header to "no-store".
    /// Ignores the "Location" parameter for values other than "None".
    /// Ignores the "duration" parameter.
    /// </summary>
    public bool NoStore
    {
        get => _noStore ?? false;
        set => _noStore = value;
    }

    /// <summary>
    /// Gets or sets the value for the Vary response header.
    /// </summary>
    public string? VaryByHeader { get; set; }

    /// <summary>
    /// Gets or sets the query keys to vary by.
    /// </summary>
    /// <remarks>
    /// <see cref="VaryByQueryKeys"/> requires the response cache middleware.
    /// </remarks>
    public string[] VaryByQueryKeys { get; set; } = [];

    /// <inheritdoc />
    public int Order { get; set; }

    /// <inheritdoc />
    public void OnActionExecuting(ActionExecutingContext context)
    {
        // Duration MUST be set unless NoStore is true.
        if (_noStore is null or false && _duration == null)
        {
            throw new InvalidOperationException($"If the '{nameof(NoStore)}' property is not set to true, '{nameof(Duration)}' property must be specified.");
        }

        if (_serverDuration != null && Location != ResponseCacheLocation.Any)
        {
            throw new InvalidOperationException($"If the '{nameof(ServerDuration)}' property is set, '{nameof(Location)}' property set to Any.");
        }

        ICacheManager cacheManager = context.HttpContext.RequestServices.GetRequiredService<ICacheManager>();
        if (!cacheManager.ShouldRequestBeCached(context.HttpContext))
        {
            return;
        }

        // VaryByQueryKeys
        IResponseCachingFeature? responseCachingFeature = context.HttpContext.Features.Get<IResponseCachingFeature>();
        if (responseCachingFeature != null)
        {
            responseCachingFeature.VaryByQueryKeys = VaryByQueryKeys;
        }

        // Using typed headers where available as it makes easier
        ResponseHeaders? typedHeaders = context.HttpContext.Response.GetTypedHeaders();
        typedHeaders.LastModified = cacheManager.LastCacheFlush;
        typedHeaders.CacheControl = new CacheControlHeaderValue
        {
            NoStore = NoStore,
            Public = Location == ResponseCacheLocation.Any,
            Private = Location == ResponseCacheLocation.Client,
            NoCache = Location == ResponseCacheLocation.None,

            // Browser Cache Duration (also used by servers if SharedMaxAge not set)
            MaxAge = _duration == null ? null : TimeSpan.FromSeconds(_duration.Value),

            // Server/CDN Cache Duration
            SharedMaxAge = _serverDuration == null ? null : TimeSpan.FromSeconds(_serverDuration.Value),
        };

        IHeaderDictionary headers = context.HttpContext.Response.Headers;
        _ = headers.Remove(HeaderNames.Vary);
        _ = headers.Remove(HeaderNames.Pragma);

        if (!string.IsNullOrEmpty(VaryByHeader))
        {
            headers[HeaderNames.Vary] = VaryByHeader;
        }

        if (Location == ResponseCacheLocation.None)
        {
            headers[HeaderNames.Pragma] = "no-cache";
        }
    }

    /// <inheritdoc />
    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Nothing to do
    }
}
