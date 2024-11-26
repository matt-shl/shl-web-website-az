using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Manifest;

namespace DTNL.UmbracoCms.Web.Modules.Contentment;

/// <summary>
/// Workaround for issue with contentment
/// See https://our.umbraco.com/packages/backoffice-extensions/contentment/contentment-feedback/112936-angular-error-after-upgrade
/// </summary>
public sealed class ContentmentComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder
            .ManifestFilters()
            .Append<ContentmentManifestFilter>();
    }
}

public sealed class ContentmentManifestFilter : IManifestFilter
{
    public void Filter(List<PackageManifest> manifests)
    {
        if (manifests.FirstOrDefault(m => m.PackageId == "Umbraco.Community.Contentment") is { } manifest)
        {
            manifest.BundleOptions = BundleOptions.Independent;
        }
    }
}
