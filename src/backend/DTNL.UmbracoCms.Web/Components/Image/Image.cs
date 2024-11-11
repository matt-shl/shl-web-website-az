using DEPT.Umbraco.SourceGenerators.CssBreakpoints;
using DTNL.UmbracoCms.Web.Helpers;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

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
        MediaWithCrops? mediaWithCrops,
        ImageCropMode imageCropMode = ImageCropMode.Crop,
        int width = 0,
        int height = 0,
        string? cssClasses = "",
        bool objectFit = true,
        IEnumerable<SrcSetEntry>? customSrcSet = null,
        string? style = null)
    {
        if (mediaWithCrops?.Content == null)
        {
            return null;
        }

        return Create(
            mediaWithCrops.Content,
            imageCropMode,
            width,
            height,
            cssClasses,
            objectFit,
            customSrcSet,
            mediaWithCrops.LocalCrops.Crops?.Select(c => ImageCrop.Create(mediaWithCrops, c)).WhereNotNull(),
            style
        );
    }

    public static Image? Create(
        IPublishedContent? imageContent,
        ImageCropMode imageCropMode = ImageCropMode.Crop,
        int width = 0,
        int height = 0,
        string? cssClasses = "",
        bool objectFit = true,
        IEnumerable<SrcSetEntry>? customSrcSet = null,
        IEnumerable<ImageCrop>? localCrops = null,
        string? style = null)
    {
        return imageContent switch
        {
            Umbraco.Cms.Web.Common.PublishedModels.Image image => Create(image, imageCropMode, width, height, cssClasses, objectFit, customSrcSet, localCrops, style),
            Umbraco.Cms.Web.Common.PublishedModels.UmbracoMediaVectorGraphics svg => Create(svg, width, height, cssClasses, objectFit, localCrops, style),
            Umbraco.Cms.Web.Common.PublishedModels.BrandfolderImage brandfolderImage => Create(brandfolderImage, imageCropMode, width, height, cssClasses, objectFit, customSrcSet, localCrops, style),
            _ => null,
        };
    }

    private static Image? Create(
        Umbraco.Cms.Web.Common.PublishedModels.BrandfolderImage brandfolderImage,
        ImageCropMode imageCropMode = ImageCropMode.Crop,
        int width = 0,
        int height = 0,
        string? cssClasses = "",
        bool objectFit = true,
        IEnumerable<SrcSetEntry>? customSrcSet = null,
        IEnumerable<ImageCrop>? localCrops = null,
        string? style = null)
    {
        string? url = brandfolderImage.GetDefaultCropUrl(width, height, imageCropMode: imageCropMode);
        if (string.IsNullOrEmpty(url))
        {
            return null;
        }

        try
        {
            Image img = new()
            {
                Url = url,
                Alt = brandfolderImage.ValueOrDefault(img => img.Alt, brandfolderImage.Name),
                SrcSet = customSrcSet switch
                {
                    { } srcSet when srcSet.Any() => brandfolderImage.BuildSrcSetString(customSrcSet),
                    _ => default,
                },
                Classes = cssClasses,
                ObjectFit = objectFit,
                Crops = GenerateCrops(localCrops, style),
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

    private static Image? Create(
        Umbraco.Cms.Web.Common.PublishedModels.Image image,
        ImageCropMode imageCropMode = ImageCropMode.Crop,
        int width = 0,
        int height = 0,
        string? cssClasses = "",
        bool objectFit = true,
        IEnumerable<SrcSetEntry>? customSrcSet = null,
        IEnumerable<ImageCrop>? localCrops = null,
        string? style = null)
    {
        string? url = image.GetDefaultCropUrl(width, height, imageCropMode: imageCropMode);
        if (string.IsNullOrEmpty(url))
        {
            return null;
        }

        try
        {
            Image img = new()
            {
                Url = url,
                Alt = image.ValueOrDefault(img => img.Alt, image.Name),
                SrcSet = customSrcSet switch
                {
                    { } srcSet when srcSet.Any() => image.BuildSrcSetString(customSrcSet),
                    _ => default,
                },
                Classes = cssClasses,
                ObjectFit = objectFit,
                Crops = GenerateCrops(localCrops, style),
                AspectRatio = width != default && height != default ? (width, height) : (16, 9),
                BackgroundColor = image.DominantColor,
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

    private static Image? Create(
        Umbraco.Cms.Web.Common.PublishedModels.UmbracoMediaVectorGraphics svg,
        int width = 0,
        int height = 0,
        string? cssClasses = "",
        bool objectFit = true,
        IEnumerable<ImageCrop>? localCrops = null,
        string? style = null)
    {
        string? url = svg.UmbracoFile;
        if (string.IsNullOrEmpty(url))
        {
            return null;
        }

        List<(ImageCrop ImageCrop, CssBreakpoint Breakpoint)> crops = GenerateCrops(localCrops, style);
        return new Image
        {
            Url = url,
            Alt = svg.ValueOrDefault(img => img.Alt, svg.Name),
            Classes = cssClasses,
            ObjectFit = objectFit,
            Crops = [],
            AspectRatio = GetAspectRatio(crops, width, height),
            BackgroundColor = svg.DominantColor,
        };
    }

    private static List<(ImageCrop ImageCrop, CssBreakpoint Breakpoint)> GenerateCrops(
        IEnumerable<ImageCrop>? localCrops,
        string? style = null)
    {
        if (localCrops is null)
        {
            return [];
        }

        if (style is not null)
        {
            string styleSuffix = $"-{style}";

            List<ImageCrop> styleCrops = [];

            foreach (ImageCrop localCrop in localCrops.Where(c => c.Name.InvariantEndsWith(styleSuffix)))
            {
                localCrop.Name = localCrop.Name.TrimEnd(styleSuffix);

                styleCrops.Add(localCrop);
            }

            localCrops = styleCrops;
        }
        else
        {
            localCrops = localCrops.Where(c => !c.Name.Contains('-'));
        }

        return localCrops
            .Select(imageCrop => (imageCrop, breakpoint: CssBreakpoints.GetBreakpoint(imageCrop.Name)))
            .Where(c => c.breakpoint != null)
            .OrderBy(c => c.breakpoint!.Start ?? 0)
            .Select(c => (c.imageCrop, c.breakpoint!))
            .ToList();
    }

    private static (int Width, int Height) GetAspectRatio(IReadOnlyCollection<(ImageCrop ImageCrop, CssBreakpoint Breakpoint)> crops, int width, int height)
    {
        if (GetAspectRatio(crops) is var cropAspect && cropAspect != default)
        {
            return cropAspect;
        }

        if (width != default && height != default)
        {
            return (width, height);
        }

        return (16, 9);
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
