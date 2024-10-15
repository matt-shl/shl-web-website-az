using DTNL.UmbracoCms.Web.Helpers.Extensions;
using DTNL.UmbracoCms.Web.Services.Brandfolder.Models;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Community.Contentment.DataEditors;

namespace DTNL.UmbracoCms.Web.Services.Brandfolder.DataSources;

public class BrandfolderDataSource : IDataPickerSource
{
    private readonly BrandfolderApiClient _brandfolderApiClient;

    public BrandfolderDataSource(BrandfolderApiClient brandfolderApiClient)
    {
        _brandfolderApiClient = brandfolderApiClient;
    }

    public string Name => "Brandfolders";

    public string Description => "List of Brandfolders";

    public string Icon => "icon-folder";

    public Dictionary<string, object>? DefaultValues => default;

    public IEnumerable<ConfigurationField> Fields => [];

    public string Group => "Custom";

    public OverlaySize OverlaySize => OverlaySize.Small;

    public async Task<IEnumerable<DataListItem>> GetItemsAsync(Dictionary<string, object> config, IEnumerable<string> values)
    {
        List<BrandfolderEntity?> entities = [];

        foreach (string value in values)
        {
            BrandfolderEntityResponse brandfolder = await _brandfolderApiClient.GetBrandfolder(value);

            entities.Add(brandfolder.Data);
        }

        return entities.Using(ToDataListItem);
    }

    public async Task<PagedResult<DataListItem>> SearchAsync(
        Dictionary<string, object> config,
        int pageNumber = 1,
        int pageSize = 12,
        string query = "")
    {
        BrandfolderEntitiesResponse brandfolders = await _brandfolderApiClient.FindBrandfolders(pageNumber, pageSize, query);

        int totalCount = brandfolders.Meta.TotalCount;

        return new PagedResult<DataListItem>(totalCount, pageNumber, pageSize)
        {
            Items = brandfolders.Data.Using(ToDataListItem),
        };
    }

    private DataListItem ToDataListItem(BrandfolderEntity content)
    {
        return new DataListItem
        {
            Name = content.Attributes.Name,
            Description = content.Attributes.TagLine?.RemoveHtml(),
            Icon = Icon,
            Value = content.Id,
        };
    }
}
