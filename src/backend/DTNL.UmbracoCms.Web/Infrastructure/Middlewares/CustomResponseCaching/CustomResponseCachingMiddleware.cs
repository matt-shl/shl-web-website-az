using System.Reflection;
using Microsoft.AspNetCore.ResponseCaching;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;

namespace DTNL.UmbracoCms.Web.Infrastructure.Middlewares.CustomResponseCaching;

/// <summary>
/// Enables HTTP response caching by extending <see cref="ResponseCachingMiddleware"/>.
/// </summary>
/// <remarks>Required to allow the underlying <see cref="MemoryCache"/> to be flushed.</remarks>
public class CustomResponseCachingMiddleware : ResponseCachingMiddleware
{
    public CustomResponseCachingMiddleware(
        RequestDelegate next,
        CustomResponseCachingMemoryCacheFactory memoryCacheFactory,
        IOptions<ResponseCachingOptions> options,
        ILoggerFactory loggerFactory,
        ObjectPoolProvider poolProvider
    )
        : base(next, options, loggerFactory, poolProvider)
    {
        SetMemoryCacheInstance(memoryCacheFactory.Cache);
    }

    /// <summary>
    /// Set the <see cref="IMemoryCache"/> of the <see cref="ResponseCachingMiddleware"/> to the provided instance.
    /// </summary>
    private void SetMemoryCacheInstance(IMemoryCache memoryCache)
    {
        object memoryResponseCache = GetInstanceField(typeof(ResponseCachingMiddleware), "_cache")?.GetValue(this)!;
        GetInstanceField(memoryResponseCache.GetType(), "_cache")!.SetValue(memoryResponseCache, memoryCache);
    }

    /// <summary>
    /// Reflection helper to get private fields.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S3011:Reflection should not be used to increase accessibility of classes, methods, or fields", Justification = "Necessary")]
    private static FieldInfo? GetInstanceField(Type type, string fieldName)
    {
        const BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.NonPublic;
        return type.GetField(fieldName, bindFlags);
    }
}
