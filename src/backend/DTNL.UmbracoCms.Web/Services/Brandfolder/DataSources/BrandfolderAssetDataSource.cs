using DTNL.UmbracoCms.Web.Helpers.Extensions;
using DTNL.UmbracoCms.Web.Services.Brandfolder.Models;
using Flurl;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco.Community.Contentment.DataEditors;
using Umbraco.Community.Contentment.Services;

namespace DTNL.UmbracoCms.Web.Services.Brandfolder.DataSources;

public abstract class BrandfolderAssetDataSource : IDataPickerSource
{
    private readonly BrandfolderApiClient _brandfolderApiClient;
    private readonly IContentmentContentContext _contentmentContentContext;
    private readonly IUmbracoContextAccessor _umbracoContextAccessor;

    protected BrandfolderAssetDataSource(
        BrandfolderApiClient brandfolderApiClient,
        IContentmentContentContext contentmentContentContext,
        IUmbracoContextAccessor umbracoContextAccessor)
    {
        _contentmentContentContext = contentmentContentContext;
        _umbracoContextAccessor = umbracoContextAccessor;
        _brandfolderApiClient = brandfolderApiClient;
    }

    protected abstract string[] SupportedFileTypes { get; }

    public virtual string Name => "Brandfolder Assets";

    public virtual string Description => "List of Brandfolder Assets";

    public virtual string Icon => "icon-picture";

    public virtual string DefaultImageAlias => "image";

    public Dictionary<string, object>? DefaultValues => default;

    public IEnumerable<ConfigurationField> Fields => [];

    public string Group => "Custom";

    public OverlaySize OverlaySize => OverlaySize.Small;

    public async Task<IEnumerable<DataListItem>> GetItemsAsync(Dictionary<string, object> config, IEnumerable<string> values)
    {
        List<BrandfolderEntity?> entities = [];

        foreach (string value in values)
        {
            string assetId = new Url(value).PathSegments.Last();

            BrandfolderEntityResponse brandfolderAsset = await _brandfolderApiClient.GetAsset(assetId);

            entities.Add(brandfolderAsset.Data);
        }

        return entities.Using(ToDataListItem);
    }

    public async Task<PagedResult<DataListItem>> SearchAsync(
        Dictionary<string, object> config,
        int pageNumber = 1,
        int pageSize = 12,
        string query = "")
    {
        if (GetBrandfolderId() is not { } brandfolderId)
        {
            return new PagedResult<DataListItem>(0, pageNumber, pageSize);
        }

        BrandfolderEntitiesResponse brandfolderAssets = await _brandfolderApiClient
            .FindAssets(brandfolderId, pageNumber, pageSize, query, SupportedFileTypes);

        int totalCount = brandfolderAssets.Meta.TotalCount;

        return new PagedResult<DataListItem>(totalCount, pageNumber, pageSize)
        {
            Items = brandfolderAssets.Data.Using(ToDataListItem),
        };
    }

    private string? GetBrandfolderId()
    {
        if (!_umbracoContextAccessor.TryGetUmbracoContext(out IUmbracoContext? umbracoContext))
        {
            return null;
        }

        if (_contentmentContentContext.GetCurrentContentId() is not { } mediaId)
        {
            return null;
        }

        IPublishedContent? content = umbracoContext.Media?.GetById(true, mediaId);

        return (content as BrandfolderFolder)?.FolderId;
    }

    private DataListItem ToDataListItem(BrandfolderEntity content)
    {
        return new DataListItem
        {
            Name = content.Attributes.Name,
            Description = content.Attributes.Description,
            Icon = Icon,
            Properties = new Dictionary<string, object>
            {
                { DefaultImageAlias, content.Attributes.ThumbnailUrl! },
            },
            Value = $"https://cdn.bfldr.com/DTG6CG68/as/{content.Id}/{content.Id}"
                .SetQueryParam("height", 350)
                .SetQueryParam("width", 350),
        };
    }
}
