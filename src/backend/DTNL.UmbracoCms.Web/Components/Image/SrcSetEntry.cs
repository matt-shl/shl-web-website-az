using DTNL.UmbracoCms.Web.Helpers.Extensions;
using DTNL.UmbracoCms.Web.Models.BrandfolderAssets;

namespace DTNL.UmbracoCms.Web.Components;

public partial class Image
{
    public class SrcSetEntry
    {
        public int Breakpoint { get; private init; }

        public int? Width { get; private set; }

        public int? Height { get; private set; }

        public double? HeightRatio { get; private set; }

        public static SrcSetEntry At(int breakpoint)
        {
            return new SrcSetEntry
            {
                Breakpoint = breakpoint,
            };
        }

        public SrcSetEntry WithWidth(int? width)
        {
            Width = width;
            return this;
        }

        public SrcSetEntry WithHeight(int? height)
        {
            Height = height;
            return this;
        }

        public SrcSetEntry WithHeightRatio(double? heightRatio)
        {
            HeightRatio = heightRatio;
            return this;
        }

        public string ToString(BrandfolderAsset brandfolderAsset)
        {
            int imageWidth = Width ?? Breakpoint;

            string imageUrl = $"{brandfolderAsset.GetDefaultCropUrl(imageWidth, Height)}";

            return $"{imageUrl} {Breakpoint}w";
        }
    }
}
