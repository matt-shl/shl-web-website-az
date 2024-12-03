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
    protected BrandfolderBaseDataSource(
        BrandfolderApiClient brandfolderApiClient,
        IContentmentContentContext contentmentContentContext,
        IUmbracoContextAccessor umbracoContextAccessor)
    {
        ContentmentContentContext = contentmentContentContext;
        UmbracoContextAccessor = umbracoContextAccessor;
        BrandfolderApiClient = brandfolderApiClient;
    }

    protected BrandfolderApiClient BrandfolderApiClient { get; }

    protected IContentmentContentContext ContentmentContentContext { get; }

    protected IUmbracoContextAccessor UmbracoContextAccessor { get; }

    public abstract string? Name { get; }

    public abstract string? Description { get; }

    public abstract string Icon { get; }

    public abstract Dictionary<string, object>? DefaultValues { get; }

    public abstract IEnumerable<ConfigurationField> Fields { get; }

    public abstract string? Group { get; }

    public abstract OverlaySize OverlaySize { get; }

    public async Task<IEnumerable<DataListItem>> GetItemsAsync(Dictionary<string, object> config, IEnumerable<string> values)
    {
        List<BrandfolderEntity> entities = [];

        foreach (string? value in values.OrEmptyIfNull())
        {
            BrandfolderEntityResponse? brandfolderAsset = await GetItem(value);

            if (brandfolderAsset?.Data is not null)
            {
                entities.Add(brandfolderAsset.Data);
            }
        }

        return await ToDataListItems(entities);
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
            Items = await ToDataListItems(brandfolderEntitiesResponse.Data),
        };
    }

    protected abstract Task<BrandfolderEntityResponse?> GetItem(string? value);

    protected abstract Task<BrandfolderEntitiesResponse?> SearchItems(int pageNumber = 1, int pageSize = 12, string query = "");

    protected virtual Task<DataListItem[]> ToDataListItems(List<BrandfolderEntity> brandfolderEntities)
    {
        IEnumerable<Task<DataListItem>> toDataListItemTasks = brandfolderEntities.Select(ToDataListItem);

        return Task.WhenAll(toDataListItemTasks);
    }

    protected virtual Task<DataListItem> ToDataListItem(BrandfolderEntity brandfolderEntity)
    {
        return Task.FromResult(new DataListItem
        {
            Name = brandfolderEntity.Attributes.Name,
            Description = brandfolderEntity.Attributes.Description ?? brandfolderEntity.Attributes.TagLine?.RemoveHtml(),
            Icon = Icon,
            Value = brandfolderEntity.Id,
        });
    }
}
