using DTNL.UmbracoCms.Web.Infrastructure.Configuration.Options;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Options;

namespace DTNL.UmbracoCms.Web.Services.Assets;

public class DevelopmentFallbackAssetsProvider : IAssetsProvider
{
    private readonly IAssetsProvider _assetsProvider;
    private readonly ILogger<DevelopmentFallbackAssetsProvider> _logger;
    private readonly IOptionsMonitor<DevelopmentOptions> _developmentOptions;

    public DevelopmentFallbackAssetsProvider(
        IAssetsProvider assetsProvider,
        ILogger<DevelopmentFallbackAssetsProvider> logger,
        IOptionsMonitor<DevelopmentOptions> options)
    {
        _assetsProvider = assetsProvider;
        _logger = logger;
        _developmentOptions = options;
    }

    public async Task<string?> GetContent(string path)
    {
        string? content = await _assetsProvider.GetContent(path);
        if (!string.IsNullOrEmpty(content))
        {
            return content;
        }

        Uri? assetsFallbackUri = _developmentOptions.CurrentValue.AssetsFallbackUri;
        if (assetsFallbackUri is null)
        {
            return null;
        }

        try
        {
            return await assetsFallbackUri
                .AppendPathSegment(path)
                .WithTimeout(3)
                .GetStringAsync();
        }
        catch (FlurlHttpException e) when (e.StatusCode == StatusCodes.Status404NotFound)
        {
            return string.Empty;
        }
        catch (FlurlHttpException e)
        {
            _logger.LogError(e, "Exception occurred while retrieving asset '{Path}' from '{FallbackUri}'.", path, assetsFallbackUri);
            return null;
        }
    }
}
