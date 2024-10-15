using DTNL.UmbracoCms.Web.Infrastructure.DependencyInjection;
using DTNL.UmbracoCms.Web.Services.Brandfolder.Models;
using Flurl;
using Flurl.Http;

namespace DTNL.UmbracoCms.Web.Services.Brandfolder;

[Singleton]
public class BrandfolderApiClient
{
    private readonly BrandfolderOptions _brandfolderOptions = new BrandfolderOptions { ApiKey = "eyJhbGciOiJIUzI1NiJ9.eyJvcmdhbml6YXRpb25fa2V5IjoiNzdnanh4NW05cnE4cDh2c24zOXJycyIsImlhdCI6MTcyNTQyNjQ1OSwidXNlcl9rZXkiOiJ2Z3F4Nmpwbms3dzc1eDg1d3ZwcDkiLCJzdXBlcnVzZXIiOmZhbHNlfQ.4S1pYbxWMiAGjB4it5E0GiqoEMrqguEEjMsoMy0yEss" };

    // TODO
    //public BrandfolderApiClient(IOptions<BrandfolderOptions> brandfolderOptions)
    //{
    //    _brandfolderOptions = brandfolderOptions.Value;
    //}

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
