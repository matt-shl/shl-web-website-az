using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Web;
using Umbraco.Community.Contentment.DataEditors;
using Umbraco.Community.Contentment.Services;

namespace DTNL.UmbracoCms.Web.Services.Brandfolder.DataSources;

public class BrandfolderSvgDataSource : BrandfolderAssetDataSource
{
    public BrandfolderSvgDataSource(
        BrandfolderApiClient brandfolderApiClient,
        IContentmentContentContext contentmentContentContext,
        IUmbracoContextAccessor umbracoContextAccessor)
        : base(brandfolderApiClient, contentmentContentContext, umbracoContextAccessor)
    {
    }

    protected override string[]? SupportedFileTypes => ["svg"];

    public override string Name => "Brandfolder SVGs";

    public override string Description => "List of Brandfolder SVGs";

    public override string Icon => "icon-picture";

    public override Dictionary<string, object>? DefaultValues => default;

    public override IEnumerable<ConfigurationField> Fields => [];

    public override string Group => "Custom";

    public override OverlaySize OverlaySize => OverlaySize.Small;
}
