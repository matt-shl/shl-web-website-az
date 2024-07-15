using DTNL.UmbracoCms.Web.Infrastructure.Configuration.Options;
using DTNL.UmbracoCms.Web.Infrastructure.Middlewares.CustomResponseCaching;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Web;

namespace DTNL.UmbracoCms.Web.Infrastructure;

/// <summary>
/// Manager for the multiple caches being used.
/// </summary>
public interface ICacheManager
{
    /// <summary>
    /// Gets the time of the last cache flush.
    /// </summary>
    DateTimeOffset LastCacheFlush { get; }

    /// <summary>
    /// Implements the logic to determine if a request should be cached.
    /// </summary>
    bool ShouldRequestBeCached(HttpContext httpContext);

    /// <summary>
    /// Flushes the caches.
    /// </summary>
    void Flush();
}

/// <inheritdoc cref="ICacheManager" />
public sealed class CacheManager : ICacheManager, IDisposable
{
    private readonly CustomResponseCachingMemoryCacheFactory _responseCachingFactory;
    private readonly CacheTagHelperMemoryCacheFactory _cacheTagHelperFactory;
    private readonly IOptionsMonitor<CacheOptions> _cacheOptions;
    private readonly IUmbracoContextAccessor _umbracoContextAccessor;
    private readonly IDisposable? _changeTracker;

    public CacheManager(
        CustomResponseCachingMemoryCacheFactory responseCachingFactory,
        CacheTagHelperMemoryCacheFactory cacheTagHelperFactory,
        IOptionsMonitor<CacheOptions> cacheOptions,
        IUmbracoContextAccessor umbracoContextAccessor
    )
    {
        _responseCachingFactory = responseCachingFactory;
        _cacheTagHelperFactory = cacheTagHelperFactory;
        _cacheOptions = cacheOptions;
        _umbracoContextAccessor = umbracoContextAccessor;

        _changeTracker = _cacheOptions.OnChange(ApplicationOptionsOnChange);
    }

    public DateTimeOffset LastCacheFlush { get; private set; } = DateTimeOffset.Now;

    public bool ShouldRequestBeCached(HttpContext httpContext)
    {
        if (!_cacheOptions.CurrentValue.Enabled)
        {
            return false;
        }

        if (httpContext.Request.IsClientSideRequest() || httpContext.Request.IsBackOfficeRequest())
        {
            return false;
        }

        if (_umbracoContextAccessor.TryGetUmbracoContext(out IUmbracoContext? umbracoContext) && (umbracoContext.InPreviewMode || umbracoContext.IsDebug))
        {
            return false;
        }

        return true;
    }

    public void Flush()
    {
        // Clear all memory caches
        _responseCachingFactory.Cache.Compact(1.0);
        (_cacheTagHelperFactory.Cache as MemoryCache)?.Compact(1.0);

        // Reset LastModified Header
        LastCacheFlush = DateTimeOffset.Now;
    }

    public void Dispose()
    {
        _changeTracker?.Dispose();
        Flush();
    }

    private void ApplicationOptionsOnChange(CacheOptions options)
    {
        if (!options.Enabled)
        {
            Flush();
        }
    }
}
