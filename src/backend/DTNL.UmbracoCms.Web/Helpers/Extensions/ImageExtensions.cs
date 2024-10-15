using DTNL.UmbracoCms.Web.Components;
using Flurl;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

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
    /// Retrieves the default crop url for an image crop.
    /// </summary>
    public static string GetDefaultCropUrl(
        this MediaWithCrops image,
        string cropAlias,
        int quality = 80,
        UrlMode urlMode = UrlMode.Default)
    {
        if (image.LocalCrops.GetCrop(cropAlias) is not { } crop)
        {
            return "";
        }

        Image.ImageCropDimensions dimensions = image.Content.GetImageCropDimensions(crop.Width, crop.Height);

        string? cropUrl = image.GetCropUrl(dimensions.Width, dimensions.Height, cropAlias: cropAlias, quality: quality, imageCropMode: ImageCropMode.Crop, urlMode: urlMode);

        // If we are dealing with a crop that has coordinates set, we don't need to worry about cropping.
        // So we just ensure we don't upscale by using crop mode Min, by overriding it.
        if (cropUrl is not null && crop.Coordinates is not null)
        {
            cropUrl = cropUrl.SetQueryParam("rmode", nameof(ImageCropMode.Min).ToLowerInvariant());
        }

        return cropUrl ?? image.MediaUrl();
    }

    /// <summary>
    /// Creates a data src set string based on custom breakpoints.
    /// </summary>
    public static string BuildSrcSetString(this Umbraco.Cms.Web.Common.PublishedModels.Image image, IEnumerable<Image.SrcSetEntry> entries)
    {
        return string.Join(",", entries.Select(x => x.ToString(image)));
    }

    private static Image.ImageCropDimensions GetImageCropDimensions(this IPublishedContent node, int? width, int? height)
    {
        int currentWidth = width ?? 0;
        int currentHeight = height ?? 0;

        if (node is not Umbraco.Cms.Web.Common.PublishedModels.Image img || (img.UmbracoWidth >= currentWidth && img.UmbracoHeight >= currentHeight))
        {
            return new Image.ImageCropDimensions
            {
                Width = currentWidth,
                Height = currentHeight,
            };
        }

        if (currentWidth == 0)
        {
            return new Image.ImageCropDimensions
            {
                Width = 0,
                Height = Math.Min(currentHeight, img.UmbracoHeight),
            };
        }

        if (currentHeight == 0)
        {
            return new Image.ImageCropDimensions
            {
                Width = Math.Min(currentWidth, img.UmbracoWidth),
                Height = 0,
            };
        }

        double ratio = currentWidth / (double) currentHeight;
        int maxWidth = Math.Min(img.UmbracoWidth, currentWidth);
        double maxHeight = Math.Min(img.UmbracoHeight, maxWidth / ratio);
        int newWidth = (int) Math.Round(maxHeight * ratio);
        int newHeight = (int) Math.Round(maxHeight);

        return new Image.ImageCropDimensions
        {
            Width = newWidth,
            Height = newHeight,
        };
    }
}
