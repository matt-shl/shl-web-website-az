using DTNL.UmbracoCms.Web.Components;
using Flurl;
using Umbraco.Cms.Core.Models;

namespace DTNL.UmbracoCms.Web.Helpers.Extensions;

public static class BrandfolderImageExtensions
{
    /// <summary>
    /// Retrieves the default crop url for an image.
    /// </summary>
    public static string? GetDefaultCropUrl(
        this Umbraco.Cms.Web.Common.PublishedModels.BrandfolderImage image,
        int? width = null,
        int? height = null,
        ImageCropMode imageCropMode = ImageCropMode.Crop)
    {
        return image.BrandfolderUrl
            .SetQueryParam("width", width is 0 ? null : width)
            .SetQueryParam("height", height is 0 ? null : height)
            .SetQueryParam("fit", GetFitParameter(imageCropMode));
    }

    /// <summary>
    /// Creates a data src set string based on custom breakpoints.
    /// </summary>
    public static string BuildSrcSetString(this Umbraco.Cms.Web.Common.PublishedModels.BrandfolderImage image, IEnumerable<Image.SrcSetEntry> entries)
    {
        return string.Join(",", entries.Select(x => x.ToString(image)));
    }

    private static string GetFitParameter(ImageCropMode cropMode)
    {
        return cropMode switch
        {
            ImageCropMode.Min => "bounds",
            ImageCropMode.Max => "cover",
            _ => "crop",
        };
    }
}
