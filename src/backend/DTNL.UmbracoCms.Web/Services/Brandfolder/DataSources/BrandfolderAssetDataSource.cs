using System.Text.Json;
using DTNL.UmbracoCms.Web.Models.BrandfolderAssets;
using DTNL.UmbracoCms.Web.Services.Brandfolder.Models;
using Flurl;
using Umbraco.Cms.Core.Web;
using Umbraco.Community.Contentment.DataEditors;
using Umbraco.Community.Contentment.Services;

namespace DTNL.UmbracoCms.Web.Services.Brandfolder.DataSources;

public abstract class BrandfolderAssetDataSource : BrandfolderBaseDataSource
{
    protected BrandfolderAssetDataSource(
        BrandfolderApiClient brandfolderApiClient,
        IContentmentContentContext contentmentContentContext,
        IUmbracoContextAccessor umbracoContextAccessor)
        : base(brandfolderApiClient, contentmentContentContext, umbracoContextAccessor)
    {
    }

    protected abstract string[]? SupportedFileTypes { get; }

    public virtual string DefaultImageAlias => "image";

    protected override async Task<BrandfolderEntityResponse?> GetItem(string? value)
    {
        try
        {
            if (BrandfolderAsset.Create(value) is not { } brandfolderAsset)
            {
                return null;
            }

            return await BrandfolderApiClient.GetAsset(brandfolderAsset.Id);
        }
        catch (Exception)
        {
            return null;
        }
    }

    protected override async Task<BrandfolderEntitiesResponse?> SearchItems(
        int pageNumber = 1,
        int pageSize = 12,
        string query = "")
    {
        return await BrandfolderApiClient.FindAssets(pageNumber, pageSize, query, SupportedFileTypes);
    }

    protected override DataListItem ToDataListItem(BrandfolderEntity brandfolderEntity)
    {
        BrandfolderAsset brandfolderAsset = new()
        {
            Id = brandfolderEntity.Id,
            Url = brandfolderEntity.Attributes.CdnUrl.RemoveQuery(),
            Name = brandfolderEntity.Attributes.Name,
        };

        return new DataListItem
        {
            Name = brandfolderEntity.Attributes.Name,
            Description = brandfolderEntity.Attributes.Description,
            Icon = Icon,
            Properties = new Dictionary<string, object> { { DefaultImageAlias, brandfolderEntity.Attributes.ThumbnailUrl! }, },
            Value = JsonSerializer.Serialize(brandfolderAsset),
        };
    }
}
