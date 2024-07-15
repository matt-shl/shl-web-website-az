using Microsoft.AspNetCore.ResponseCaching;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace DTNL.UmbracoCms.Web.Infrastructure.Middlewares.CustomResponseCaching;

/// <summary>
/// A factory for <see cref="MemoryCache"/> configured using <see cref="ResponseCachingOptions"/>.
/// <see cref="CustomResponseCachingMiddleware"/> uses this factory to set its <see cref="IMemoryCache"/>.
/// </summary>
public class CustomResponseCachingMemoryCacheFactory
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomResponseCachingMemoryCacheFactory"/> class.
    /// </summary>
    /// <param name="options">The <see cref="ResponseCachingOptions"/> to apply to the <see cref="Cache"/>.</param>
    public CustomResponseCachingMemoryCacheFactory(IOptions<ResponseCachingOptions> options)
    {
        Cache = new MemoryCache(new MemoryCacheOptions
        {
            SizeLimit = options.Value.SizeLimit,
        });
    }

    /// <summary>
    /// Gets the <see cref="MemoryCache"/>.
    /// </summary>
    public MemoryCache Cache { get; }
}
