using DTNL.UmbracoCms.Web.Services.Brandfolder.Models;
using Flurl;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.PublishedModels;
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

    protected abstract string[] SupportedFileTypes { get; }

    public virtual string DefaultImageAlias => "image";

    protected override async Task<BrandfolderEntityResponse> GetItem(string value)
    {
        string assetId = new Url(value).PathSegments.Last();

        return await BrandfolderApiClient.GetAsset(assetId);
    }

    protected override async Task<BrandfolderEntitiesResponse?> SearchItems(
        int pageNumber = 1,
        int pageSize = 12,
        string query = "")
    {
        if (GetBrandfolderSectionId() is { } brandfolderSectionId)
        {
            return await BrandfolderApiClient
                .FindSectionAssets(brandfolderSectionId, pageNumber, pageSize, query, SupportedFileTypes);
        }

        if (GetBrandfolderId() is { } brandfolderId)
        {
            return await BrandfolderApiClient
                .FindAssets(brandfolderId, pageNumber, pageSize, query, SupportedFileTypes);
        }

        return null;
    }

    protected string? GetBrandfolderSectionId()
    {
        if (!UmbracoContextAccessor.TryGetUmbracoContext(out IUmbracoContext? umbracoContext))
        {
            return null;
        }

        if (ContentmentContentContext.GetCurrentContentId() is not { } mediaId)
        {
            return null;
        }

        IPublishedContent? content = umbracoContext.Media?.GetById(true, mediaId);

        return (content as BrandfolderFolder)?.SectionId;
    }

    protected override DataListItem ToDataListItem(BrandfolderEntity content)
    {
        return new DataListItem
        {
            Name = content.Attributes.Name,
            Description = content.Attributes.Description,
            Icon = Icon,
            Properties = new Dictionary<string, object> { { DefaultImageAlias, content.Attributes.ThumbnailUrl! }, },
            Value = $"https://cdn.bfldr.com/DTG6CG68/as/{content.Id}/{content.Id}"
                .SetQueryParam("height", 250)
                .SetQueryParam("width", 250)
                .SetQueryParam("fit", "crop"),
        };
    }
}
