using DTNL.UmbracoCms.Web.Models.BrandfolderAssets;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class DownloadItem
{
    public required string Title { get; set; }

    public string? Description { get; set; }

    public string? Icon { get; set; }

    public required DownloadOverlay DownloadOverlay { get; set; }

    public static DownloadItem? Create(IPublishedElement item, SiteSettings? settings)
    {
        if (item is not NestedBlockDownloadItem downloadItem ||
            DownloadOverlay.Create(downloadItem, settings) is not { } downloadOverlay)
        {
            return null;
        }

        return new DownloadItem
        {
            Title = downloadItem.Title!,
            Description = downloadItem.Description,
            Icon = BrandfolderAsset.GetAssetUrl(downloadItem.Icon),
            DownloadOverlay = downloadOverlay,
        };
    }
}
