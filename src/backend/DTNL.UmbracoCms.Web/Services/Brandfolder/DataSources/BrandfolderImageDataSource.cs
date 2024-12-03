using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Web;
using Umbraco.Community.Contentment.DataEditors;
using Umbraco.Community.Contentment.Services;

namespace DTNL.UmbracoCms.Web.Services.Brandfolder.DataSources;

public class BrandfolderImageDataSource : BrandfolderAssetDataSource
{
    public BrandfolderImageDataSource(
        BrandfolderApiClient brandfolderApiClient,
        IContentmentContentContext contentmentContentContext,
        IUmbracoContextAccessor umbracoContextAccessor)
        : base(brandfolderApiClient, contentmentContentContext, umbracoContextAccessor)
    {
    }

    protected override string[] SupportedFileTypes => ["jpg", "gif", "png", "tif"];

    public override string Name => "Brandfolder Images";

    public override string Description => "List of Brandfolder Images";

    public override string Icon => "icon-picture";

    public override Dictionary<string, object>? DefaultValues => default;

    public override IEnumerable<ConfigurationField> Fields => [];

    public override string Group => "Custom";

    public override OverlaySize OverlaySize => OverlaySize.Large;
}
