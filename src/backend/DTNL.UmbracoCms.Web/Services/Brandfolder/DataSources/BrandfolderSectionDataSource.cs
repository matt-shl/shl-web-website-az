using DTNL.UmbracoCms.Web.Services.Brandfolder.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Web;
using Umbraco.Community.Contentment.DataEditors;
using Umbraco.Community.Contentment.Services;

namespace DTNL.UmbracoCms.Web.Services.Brandfolder.DataSources;

public class BrandfolderSectionDataSource : BrandfolderBaseDataSource
{
    public BrandfolderSectionDataSource(
        BrandfolderApiClient brandfolderApiClient,
        IContentmentContentContext contentmentContentContext,
        IUmbracoContextAccessor umbracoContextAccessor)
        : base(brandfolderApiClient, contentmentContentContext, umbracoContextAccessor)
    {
    }

    public override string Name => "Brandfolder Section";

    public override string Description => "List of Brandfolder Section";

    public override string Icon => "icon-bookmark";

    public override Dictionary<string, object>? DefaultValues => default;

    public override IEnumerable<ConfigurationField> Fields => [];

    public override string Group => "Custom";

    public override OverlaySize OverlaySize => OverlaySize.Small;

    protected override async Task<BrandfolderEntityResponse> GetItem(string value)
    {
        return await BrandfolderApiClient.GetBrandfolderSection(value);
    }

    protected override async Task<BrandfolderEntitiesResponse?> SearchItems(int pageNumber = 1, int pageSize = 12, string query = "")
    {
        return await BrandfolderApiClient.FindBrandfolderSections(pageNumber, pageSize, query);
    }
}
