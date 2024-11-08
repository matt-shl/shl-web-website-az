using Flurl;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Image = DTNL.UmbracoCms.Web.Components.Image;

namespace DTNL.UmbracoCms.Web.Helpers.Extensions;

public static class ImageCropExtensions
{
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

        string? cropUrl;
        if (image.Content is Umbraco.Cms.Web.Common.PublishedModels.BrandfolderImage brandfolderImage)
        {
            cropUrl = brandfolderImage.GetDefaultCropUrl(dimensions.Width, dimensions.Height, imageCropMode: ImageCropMode.Crop);
        }
        else
        {
            cropUrl = image.GetCropUrl(dimensions.Width, dimensions.Height, cropAlias: cropAlias, quality: quality, imageCropMode: ImageCropMode.Crop, urlMode: urlMode);

            // If we are dealing with a crop that has coordinates set, we don't need to worry about cropping.
            // So we just ensure we don't upscale by using crop mode Min, by overriding it.
            if (cropUrl is not null && crop.Coordinates is not null)
            {
                cropUrl = cropUrl.SetQueryParam("rmode", nameof(ImageCropMode.Min).ToLowerInvariant());
            }
        }

        return cropUrl ?? image.MediaUrl();
    }

    public static Image.ImageCropDimensions GetImageCropDimensions(this IPublishedContent? node, int? width, int? height)
    {
        int currentWidth = width ?? 0;
        int currentHeight = height ?? 0;

        return node switch
        {
            Umbraco.Cms.Web.Common.PublishedModels.Image image => GetImageCropDimensions(image, currentWidth, currentHeight),
            Umbraco.Cms.Web.Common.PublishedModels.BrandfolderImage brandfolderImage => GetImageCropDimensions(brandfolderImage, currentWidth, currentHeight),
            _ => new Image.ImageCropDimensions { Width = currentWidth, Height = currentHeight, },
        };
    }

    public static Image.ImageCropDimensions GetImageCropDimensions(this Umbraco.Cms.Web.Common.PublishedModels.Image image, int currentWidth, int currentHeight)
    {
        if (image.UmbracoWidth >= currentWidth && image.UmbracoHeight >= currentHeight)
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
                Height = Math.Min(currentHeight, image.UmbracoHeight),
            };
        }

        if (currentHeight == 0)
        {
            return new Image.ImageCropDimensions
            {
                Width = Math.Min(currentWidth, image.UmbracoWidth),
                Height = 0,
            };
        }

        double ratio = currentWidth / (double) currentHeight;
        int maxWidth = Math.Min(image.UmbracoWidth, currentWidth);
        double maxHeight = Math.Min(image.UmbracoHeight, maxWidth / ratio);
        int newWidth = (int) Math.Round(maxHeight * ratio);
        int newHeight = (int) Math.Round(maxHeight);

        return new Image.ImageCropDimensions
        {
            Width = newWidth,
            Height = newHeight,
        };
    }

    public static Image.ImageCropDimensions GetImageCropDimensions(this Umbraco.Cms.Web.Common.PublishedModels.BrandfolderImage brandfolderImage, int currentWidth, int currentHeight)
    {
        return new Image.ImageCropDimensions
        {
            Width = currentWidth,
            Height = currentHeight,
        };
    }
}
