using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace DTNL.UmbracoCms.Web.Services.Assets;

public class CachedAssetsProvider : IAssetsProvider
{
    private readonly IAssetsProvider _assetsProvider;
    private readonly IMemoryCache _memoryCache;

    private static readonly MemoryCacheEntryOptions DefaultCacheEntryOptions = new()
    {
        SlidingExpiration = TimeSpan.FromMinutes(60),
    };

    public CachedAssetsProvider(IAssetsProvider assetsProvider, IMemoryCache memoryCache)
    {
        _assetsProvider = assetsProvider;
        _memoryCache = memoryCache;
    }

    public async Task<string?> GetContent(string path)
    {
        string key = _memoryCache.GetKey<CachedAssetsProvider>(path);

        string? cachedValue = _memoryCache.Get<string?>(key);
        if (cachedValue != null)
        {
            return cachedValue;
        }

        string? content;

        if (Uri.TryCreate(path, UriKind.Absolute, out Uri? _))
        {
            content = await ExternalAssetsProvider.GetContent(path);
        }
        else
        {
            content = await _assetsProvider.GetContent(path);
        }

        if (content != null)
        {
            _ = _memoryCache.Set(key, content, DefaultCacheEntryOptions);
        }

        return content;
    }
}
