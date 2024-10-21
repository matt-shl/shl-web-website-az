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

    public async Task<BrandfolderEntityResponse> GetBrandfolder(string brandfolderId)
    {
        return await $"https://brandfolder.com/api/v4/brandfolders/{brandfolderId}"
            .WithOAuthBearerToken(_brandfolderOptions.ApiKey)
            .GetJsonAsync<BrandfolderEntityResponse>();
    }

    public async Task<BrandfolderEntitiesResponse> FindBrandfolders(int page, int pageSize, string? searchQuery)
    {
        return await "https://brandfolder.com/api/v4/brandfolders"
            .SetQueryParam("search", searchQuery)
            .SetQueryParam("page", page)
            .SetQueryParam("per", pageSize)
            .WithOAuthBearerToken(_brandfolderOptions.ApiKey)
            .GetJsonAsync<BrandfolderEntitiesResponse>();
    }

    public async Task<BrandfolderEntityResponse> GetBrandfolderSection(string brandfolderSectionId)
    {
        return await $"https://brandfolder.com/api/v4/sections/{brandfolderSectionId}"
            .WithOAuthBearerToken(_brandfolderOptions.ApiKey)
            .GetJsonAsync<BrandfolderEntityResponse>();
    }

    public async Task<BrandfolderEntitiesResponse> FindBrandfolderSections(string brandfolderId, int page, int pageSize, string? searchQuery)
    {
        return await $"https://brandfolder.com/api/v4/brandfolders/{brandfolderId}/sections"
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
        string[] fileTypes)
    {
        if (!searchQuery.IsNullOrWhiteSpace())
        {
            searchQuery = $"{searchQuery} AND ";
        }

        searchQuery =
            $"{searchQuery} ({string.Join("OR ", fileTypes.Select(fileType => $"filetype.strict:\"{fileType}\""))})";

        string ir;

        return await $"https://brandfolder.com/api/v4/sections/{sectionId}/assets"
            .SetQueryParam("search", searchQuery)
            .SetQueryParam("page", page)
            .SetQueryParam("per", pageSize)
            .WithOAuthBearerToken(_brandfolderOptions.ApiKey)
            .GetJsonAsync<BrandfolderEntitiesResponse>();
    }

    public async Task<BrandfolderEntityResponse> GetAsset(string assetId)
    {
        return await $"https://brandfolder.com/api/v4/assets/{assetId}"
            .WithOAuthBearerToken(_brandfolderOptions.ApiKey)
            .GetJsonAsync<BrandfolderEntityResponse>();
    }

    public async Task<BrandfolderEntitiesResponse> FindAssets(
        string brandfolderId,
        int page,
        int pageSize,
        string? searchQuery,
        string[] fileTypes)
    {
        if (!searchQuery.IsNullOrWhiteSpace())
        {
            searchQuery = $"{searchQuery} AND ";
        }

        searchQuery =
            $"{searchQuery} ({string.Join("OR ", fileTypes.Select(fileType => $"filetype.strict:\"{fileType}\""))})";

        return await $"https://brandfolder.com/api/v4/brandfolders/{brandfolderId}/assets"
            .SetQueryParam("search", searchQuery)
            .SetQueryParam("page", page)
            .SetQueryParam("per", pageSize)
            .WithOAuthBearerToken(_brandfolderOptions.ApiKey)
            .GetJsonAsync<BrandfolderEntitiesResponse>();
    }
}
