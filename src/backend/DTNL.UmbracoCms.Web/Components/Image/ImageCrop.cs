using DTNL.UmbracoCms.Web.Helpers;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using DTNL.UmbracoCms.Web.Models.BrandfolderAssets;
using Umbraco.Cms.Core.Models;

namespace DTNL.UmbracoCms.Web.Components;

public partial class Image
{
    public class ImageCrop
    {
        public required string Name { get; set; }

        public required ImageCropDimensions Dimensions { get; set; }

        public required string Url { get; set; }

        public static ImageCrop Create(BrandfolderAttachment brandfolderAttachment, ImageCropping crop, ImageCropMode imageCropMode)
        {
            return new ImageCrop
            {
                Name = crop.Name,
                Dimensions = new ImageCropDimensions { Width = crop.Width, Height = crop.Height },
                Url = brandfolderAttachment.GetDefaultCropUrl(crop.Width, crop.Height, imageCropMode),
            };
        }
    }

    public class ImageCropDimensions
    {
        public required int Width { get; set; }

        public required int Height { get; set; }
    }
}
