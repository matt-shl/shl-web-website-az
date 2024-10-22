using DTNL.UmbracoCms.Web.Helpers.Extensions;
using DTNL.UmbracoCms.Web.Services.Brandfolder.Models;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Web;
using Umbraco.Community.Contentment.DataEditors;
using Umbraco.Community.Contentment.Services;

namespace DTNL.UmbracoCms.Web.Services.Brandfolder.DataSources;

public abstract class BrandfolderBaseDataSource : IDataPickerSource
{
    protected readonly BrandfolderApiClient BrandfolderApiClient;
    protected readonly IContentmentContentContext ContentmentContentContext;
    protected readonly IUmbracoContextAccessor UmbracoContextAccessor;

    protected BrandfolderBaseDataSource(
        BrandfolderApiClient brandfolderApiClient,
        IContentmentContentContext contentmentContentContext,
        IUmbracoContextAccessor umbracoContextAccessor)
    {
        ContentmentContentContext = contentmentContentContext;
        UmbracoContextAccessor = umbracoContextAccessor;
        BrandfolderApiClient = brandfolderApiClient;
    }

    public abstract string? Name { get; }

    public abstract string? Description { get; }

    public abstract string Icon { get; }

    public abstract Dictionary<string, object>? DefaultValues { get; }

    public abstract IEnumerable<ConfigurationField> Fields { get; }

    public abstract string? Group { get; }

    public abstract OverlaySize OverlaySize { get; }

    public async Task<IEnumerable<DataListItem>> GetItemsAsync(Dictionary<string, object> config, IEnumerable<string> values)
    {
        List<BrandfolderEntity?> entities = [];

        foreach (string value in values)
        {
            BrandfolderEntityResponse brandfolderAsset = await GetItem(value);

            entities.Add(brandfolderAsset.Data);
        }

        return entities.Using(ToDataListItem);
    }

    public async Task<PagedResult<DataListItem>> SearchAsync(Dictionary<string, object> config, int pageNumber = 1, int pageSize = 12, string query = "")
    {
        BrandfolderEntitiesResponse? brandfolderEntitiesResponse = await SearchItems(pageNumber, pageSize, query);

        if (brandfolderEntitiesResponse is null)
        {
            return new PagedResult<DataListItem>(0, pageNumber, pageSize);
        }

        int totalCount = brandfolderEntitiesResponse.Meta.TotalCount;

        return new PagedResult<DataListItem>(totalCount, pageNumber, pageSize)
        {
            Items = brandfolderEntitiesResponse.Data.Using(ToDataListItem),
        };
    }

    protected abstract Task<BrandfolderEntityResponse> GetItem(string value);

    protected abstract Task<BrandfolderEntitiesResponse?> SearchItems(int pageNumber = 1, int pageSize = 12, string query = "");

    protected virtual DataListItem ToDataListItem(BrandfolderEntity content)
    {
        return new DataListItem
        {
            Name = content.Attributes.Name,
            Description = content.Attributes.Description ?? content.Attributes.TagLine?.RemoveHtml(),
            Icon = Icon,
            Value = content.Id,
        };
    }
}
