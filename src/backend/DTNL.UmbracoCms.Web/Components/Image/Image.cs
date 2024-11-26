using DEPT.Umbraco.SourceGenerators.CssBreakpoints;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using DTNL.UmbracoCms.Web.Models.BrandfolderAssets;
using Umbraco.Cms.Core.Models;

namespace DTNL.UmbracoCms.Web.Components;

public partial class Image : ICard
{
    public string? Classes { get; set; }

    public string? Preload { get; set; }

    public required string Url { get; set; }

    public string? SrcSet { get; set; }

    public string? Alt { get; set; }

    public string? Caption { get; set; }

    public bool Hidden { get; set; }

    public required bool ObjectFit { get; set; }

    public Dictionary<string, string?> Attributes { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public string? BackgroundColor { get; set; }

    public bool ShowPreload { get; set; } = true;

    public required List<(ImageCrop ImageCrop, CssBreakpoint Breakpoint)> Crops { get; set; }

    public required (int Width, int Height) AspectRatio { get; set; }

    public bool ImageHolderButton { get; set; }

    public Dictionary<string, string?> ImageHolderAttributes { get; set; } = [];

    public CardOverlay? CardOverlay { get; set; }

    public static Image? Create(
        string? value,
        ImageCropMode imageCropMode = ImageCropMode.Crop,
        int width = 0,
        int height = 0,
        string? cssClasses = "",
        bool objectFit = true,
        string? style = null)
    {
        if (BrandfolderAsset.Create(value) is not { } brandfolderAsset)
        {
            return null;
        }

        string url = brandfolderAsset.GetDefaultCropUrl(width, height, imageCropMode);
        if (string.IsNullOrEmpty(url))
        {
            return null;
        }

        try
        {
            Image img = new()
            {
                Url = url,
                Alt = brandfolderAsset.Name,
                SrcSet = default,
                Classes = cssClasses,
                ObjectFit = objectFit,
                Crops = brandfolderAsset.GetCrops(style ?? "default", imageCropMode),
                AspectRatio = width != default && height != default ? (width, height) : (16, 9),
            };

            img.AspectRatio = GetAspectRatio(img.Crops);

            return img;
        }
        catch
        {
            // Do nothing: sometimes the crop might fail because the
            // image media item was deleted and the page property was not updated to a new media
        }

        return null;
    }

    private static (int Width, int Height) GetAspectRatio(IReadOnlyCollection<(ImageCrop ImageCrop, CssBreakpoint Breakpoint)> crops)
    {
        if (crops.Count == 0)
        {
            return default;
        }

        (ImageCrop imageCrop, _) = crops.Aggregate((curMax, x) => curMax == default || (x.Breakpoint.Priority ?? 0) > (curMax.Breakpoint.Priority ?? 0) ? x : curMax);
        return (imageCrop.Dimensions.Width, imageCrop.Dimensions.Height);
    }
}
