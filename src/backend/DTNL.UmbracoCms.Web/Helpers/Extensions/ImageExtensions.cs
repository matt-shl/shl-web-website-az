using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Image = DTNL.UmbracoCms.Web.Components.Image;

namespace DTNL.UmbracoCms.Web.Helpers.Extensions;

public static class ImageExtensions
{
    /// <summary>
    /// Retrieves the default crop url for an image.
    /// </summary>
    public static string? GetDefaultCropUrl(
        this Umbraco.Cms.Web.Common.PublishedModels.Image image,
        int? width = null,
        int? height = null,
        int quality = 80,
        UrlMode urlMode = UrlMode.Default,
        ImageCropMode imageCropMode = ImageCropMode.Crop)
    {
        Image.ImageCropDimensions dimensions = image.GetImageCropDimensions(width, height);

        return image.GetCropUrl(dimensions.Width, dimensions.Height, quality: quality, imageCropMode: imageCropMode, urlMode: urlMode);
    }

    /// <summary>
    /// Creates a data src set string based on custom breakpoints.
    /// </summary>
    public static string BuildSrcSetString(this Umbraco.Cms.Web.Common.PublishedModels.Image image, IEnumerable<Image.SrcSetEntry> entries)
    {
        return string.Join(",", entries.Select(x => x.ToString(image)));
    }
}
