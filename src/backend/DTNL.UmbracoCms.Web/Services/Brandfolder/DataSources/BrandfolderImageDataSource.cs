using Umbraco.Cms.Core.Web;
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
}
