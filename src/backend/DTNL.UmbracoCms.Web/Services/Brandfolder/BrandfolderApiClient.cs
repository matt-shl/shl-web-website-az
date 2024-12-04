using System.Text;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using DTNL.UmbracoCms.Web.Services.Brandfolder.Models;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Options;

namespace DTNL.UmbracoCms.Web.Services.Brandfolder;

public class BrandfolderApiClient
{
    private readonly BrandfolderOptions _brandfolderOptions;
    private readonly ILogger<BrandfolderApiClient> _logger;

    public BrandfolderApiClient(
        IOptions<BrandfolderOptions> brandfolderOptions,
        ILogger<BrandfolderApiClient> logger)
    {
        _brandfolderOptions = brandfolderOptions.Value;
        _logger = logger;
    }

    public async Task<BrandfolderEntityResponse> GetBrandfolderSection(string? brandfolderSectionId)
    {
        return await $"https://brandfolder.com/api/v4/sections/{brandfolderSectionId}"
            .WithOAuthBearerToken(_brandfolderOptions.ApiKey)
            .GetJsonAsync<BrandfolderEntityResponse>();
    }

    public async Task<BrandfolderEntitiesResponse> FindBrandfolderSections(int page, int pageSize, string? searchQuery)
    {
        return await $"https://brandfolder.com/api/v4/collections/{_brandfolderOptions.CollectionId}/sections"
            .SetQueryParam("search", searchQuery)
            .SetQueryParam("page", page)
            .SetQueryParam("per", pageSize)
            .WithOAuthBearerToken(_brandfolderOptions.ApiKey)
            .GetJsonAsync<BrandfolderEntitiesResponse>();
    }

    public async Task<BrandfolderEntitiesResponse> FindSectionAssets(
        string sectionId,
        int page,
        int pageSize,
        string? searchQuery,
        string[]? fileTypes)
    {
        if (!searchQuery.IsNullOrWhiteSpace())
        {
            searchQuery = $"{searchQuery}";
        }

        if (fileTypes is not null)
        {
            searchQuery =
                $"{searchQuery} AND ({string.Join("OR ", fileTypes.Select(fileType => $"filetype.strict:\"{fileType}\""))})";
        }

        return await $"https://brandfolder.com/api/v4/sections/{sectionId}/assets"
            .SetQueryParam("fields", "cdn_url")
            .SetQueryParam("search", searchQuery)
            .SetQueryParam("page", page)
            .SetQueryParam("per", pageSize)
            .WithOAuthBearerToken(_brandfolderOptions.ApiKey)
            .GetJsonAsync<BrandfolderEntitiesResponse>();
    }

    public async Task<BrandfolderEntityResponse?> GetAssetAttachment(string attachmentId)
    {
        try
        {
            return await $"https://brandfolder.com/api/v4/attachments/{attachmentId}"
                .SetQueryParam("fields", "cdn_url")
                .SetQueryParam("include", "asset")
                .WithOAuthBearerToken(_brandfolderOptions.ApiKey)
                .GetJsonAsync<BrandfolderEntityResponse>();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error finding asset attachment {Id}", attachmentId);
            return null;
        }
    }

    public async Task<BrandfolderEntityResponse?> GetAsset(string assetId)
    {
        try
        {
            return await $"https://brandfolder.com/api/v4/assets/{assetId}"
                .SetQueryParam("fields", "cdn_url")
                .WithOAuthBearerToken(_brandfolderOptions.ApiKey)
                .GetJsonAsync<BrandfolderEntityResponse>();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error finding asset {Id}", assetId);

            return null;
        }
    }

    public async Task<BrandfolderEntitiesResponse?> FindAssetAttachments(
        int page,
        int pageSize,
        string? searchQuery,
        string[]? fileTypes)
    {
        StringBuilder queryStringBuilder = new(searchQuery.FallBack(string.Empty));

        if (fileTypes is not null)
        {
            if (queryStringBuilder.Length > 0)
            {
                queryStringBuilder.Append(" AND ");
            }

            queryStringBuilder.Append($"({string.Join("OR ", fileTypes.Select(fileType => $"filetype.strict:\"{fileType}\""))})");
        }

        try
        {
            return await $"https://brandfolder.com/api/v4/collections/{_brandfolderOptions.CollectionId}/attachments"
                .SetQueryParam("fields", "cdn_url")
                .SetQueryParam("include", "asset")
                .SetQueryParam("search", queryStringBuilder.ToString())
                .SetQueryParam("page", page)
                .SetQueryParam("per", pageSize)
                .WithOAuthBearerToken(_brandfolderOptions.ApiKey)
                .GetJsonAsync<BrandfolderEntitiesResponse>();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error finding asset attachments");
            return null;
        }
    }
}
