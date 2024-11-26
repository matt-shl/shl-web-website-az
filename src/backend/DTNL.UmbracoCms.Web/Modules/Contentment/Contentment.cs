using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Manifest;

namespace DTNL.UmbracoCms.Web.Modules.Contentment;

public sealed class ContentmentComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        //builder
        //    .ManifestFilters()
        //    .Append<ContentmentManifestFilter>();
    }
}

public sealed class ContentmentManifestFilter : IManifestFilter
{
    public void Filter(List<PackageManifest> manifests)
    {
        // Find the existing manifest, or create a new one if it doesn't exist
        var manifest = manifests.FirstOrDefault(m => m.PackageId == "Umbraco.Community.Contentment");

        if (manifest != null)
        {
            // Modify the existing manifest
            manifest.BundleOptions = BundleOptions.Independent;
        }
    }
}
