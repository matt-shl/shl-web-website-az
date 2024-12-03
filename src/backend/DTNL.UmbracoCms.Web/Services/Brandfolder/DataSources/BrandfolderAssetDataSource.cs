using System.Text.Json;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
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
            if (BrandfolderAttachment.Create(value) is not { } brandfolderAsset)
            {
                return null;
            }

            return await BrandfolderApiClient.GetAssetAttachment(brandfolderAsset.Id);
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
        return await BrandfolderApiClient.FindAssetAttachments(pageNumber, pageSize, query, SupportedFileTypes);
    }

    protected override async Task<DataListItem> ToDataListItem(BrandfolderEntity brandfolderEntity)
    {
        BrandfolderAttachment brandfolderAttachment = new()
        {
            Id = brandfolderEntity.Id,
            Url = brandfolderEntity.Attributes.CdnUrl.RemoveQuery(),
            FileName = brandfolderEntity.Attributes.FileName,
        };

        if (brandfolderEntity.Relationships?.Asset?.Data is not null &&
            await BrandfolderApiClient.GetAsset(brandfolderEntity.Relationships.Asset.Data.Id) is { } brandfolderAsset)
        {
            brandfolderAttachment.AssetId = brandfolderAsset.Data?.Id;
            brandfolderAttachment.AssetName = brandfolderAsset.Data?.Attributes.Name;
            brandfolderAttachment.AssetDescription = brandfolderAsset.Data?.Attributes.Description;
        }

        return new DataListItem
        {
            Name = brandfolderAttachment.FileName,
            Description = brandfolderAttachment.AssetDescription.FallBack(brandfolderAttachment.AssetName),
            Icon = Icon,
            Properties = new Dictionary<string, object> { { DefaultImageAlias, brandfolderAttachment.GetDefaultCropUrl(262, 162) }, },
            Value = JsonSerializer.Serialize(brandfolderAttachment),
        };
    }
}
