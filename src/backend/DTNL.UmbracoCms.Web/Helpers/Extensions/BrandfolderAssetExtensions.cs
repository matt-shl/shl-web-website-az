using DEPT.Umbraco.SourceGenerators.CssBreakpoints;
using DTNL.UmbracoCms.Web.Components;
using DTNL.UmbracoCms.Web.Models.BrandfolderAssets;
using Flurl;
using Umbraco.Cms.Core.Models;

namespace DTNL.UmbracoCms.Web.Helpers.Extensions;

public static class BrandfolderAssetExtensions
{
    /// <summary>
    /// Retrieves the default crop url for an asset. If the asset is not a file, Brandfolder generates a relevant thumbnail.
    /// </summary>
    public static string GetDefaultCropUrl(
        this BrandfolderAsset brandfolderAsset,
        int? width = null,
        int? height = null,
        ImageCropMode imageCropMode = ImageCropMode.Crop)
    {
        return brandfolderAsset.Url
            .SetQueryParam("width", width is 0 ? null : width)
            .SetQueryParam("height", height is 0 ? null : height)
            .SetQueryParam("fit", GetFitParameter(imageCropMode));
    }

    /// <summary>
    /// Creates a data src set string based on custom breakpoints.
    /// </summary>
    public static List<(Image.ImageCrop ImageCrop, CssBreakpoint Breakpoint)> GetCrops(
        this BrandfolderAsset brandfolderAsset,
        string style,
        ImageCropMode imageCropMode)
    {
        ImageCropping[] imageCroppings = ImageStylesHelper.GetImageCroppings(style);

        if (imageCroppings.Length == 0)
        {
            return [];
        }

        return imageCroppings
            .Select(imageCropping =>
                (imageCrop: Image.ImageCrop.Create(brandfolderAsset, imageCropping, imageCropMode),
                    breakpoint: CssBreakpoints.GetBreakpoint(imageCropping.Name)))
            .Where(c => c.breakpoint != null)
            .OrderBy(c => c.breakpoint!.Start ?? 0)
            .Select(c => (c.imageCrop, c.breakpoint!))
            .ToList();
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
