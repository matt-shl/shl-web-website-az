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

    public BrandfolderApiClient(IOptions<BrandfolderOptions> brandfolderOptions)
    {
        _brandfolderOptions = brandfolderOptions.Value;
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

    public async Task<BrandfolderEntityResponse> GetAsset(string assetId)
    {
        return await $"https://brandfolder.com/api/v4/assets/{assetId}"
            .SetQueryParam("fields", "cdn_url")
            .WithOAuthBearerToken(_brandfolderOptions.ApiKey)
            .GetJsonAsync<BrandfolderEntityResponse>();
    }

    public async Task<BrandfolderEntitiesResponse> FindAssets(
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

        return await $"https://brandfolder.com/api/v4/collections/{_brandfolderOptions.CollectionId}/assets"
            .SetQueryParam("fields", "cdn_url")
            .SetQueryParam("search", queryStringBuilder.ToString())
            .SetQueryParam("page", page)
            .SetQueryParam("per", pageSize)
            .WithOAuthBearerToken(_brandfolderOptions.ApiKey)
            .GetJsonAsync<BrandfolderEntitiesResponse>();
    }
}
