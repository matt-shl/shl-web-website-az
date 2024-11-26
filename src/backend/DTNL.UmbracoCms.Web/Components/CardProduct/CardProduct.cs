using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class CardProduct : ICard, IOverviewItem
{
    public required string Title { get; set; }

    public required List<ProductSpecification> Specifications { get; set; }

    public string? Text { get; set; }

    public Image? Image { get; set; }

    public string? Url { get; set; }

    public string? CssClasses { get; set; }

    public bool? ShowSpecs { get; set; }

    public static CardProduct Create(PageProduct productPage, string? cssClasses = null)
    {
        NestedBlockProductBanner? banner = productPage.Banner?.GetSingleContentOrNull<NestedBlockProductBanner>();
        NestedBlockProductHero? hero = productPage.Hero?.GetSingleContentOrNull<NestedBlockProductHero>();

        Image? image = Image
            .Create(
                productPage.CardImage ?? hero?.Image,
                imageCropMode: ImageCropMode.Max,
                cssClasses: "card-product__image");

        if (image is null && banner is not null)
        {
            image = Image
                .Create(
                    banner.Image,
                    imageCropMode: ImageCropMode.Max,
                    cssClasses: "card-product__image");
        }

        return new CardProduct
        {
            Title = productPage.GetTitle(),
            Specifications = ProductSpecification.Create(productPage).ToList(),
            Text = (productPage.CardDescription?.ToHtmlString())
                .FallBack(banner?.Text?.ToHtmlString())
                .FallBack(productPage.GetDescription()),
            Image = image,
            Url = productPage.Url(),
            CssClasses = cssClasses,
        };
    }

    public class ProductSpecification
    {
        public required string Name { get; set; }

        public required string Value { get; set; }

        public static ProductSpecification? Create(ICompositionProductDetails productDetails, string specificationKey)
        {
            if (productDetails.Value<string>(specificationKey) is { } specificationValue &&
                !specificationValue.IsNullOrWhiteSpace())
            {
                return new ProductSpecification
                {
                    Name = specificationKey,
                    Value = specificationValue,
                };
            }

            return null;
        }

        public static IEnumerable<ProductSpecification> Create(ICompositionProductDetails productDetails)
        {
            if (Create(productDetails, nameof(productDetails.VolumeLevel)) is { } volumeLevelSpecification)
            {
                yield return volumeLevelSpecification;
            }

            if (Create(productDetails, nameof(productDetails.ViscosityLevel)) is { } viscosityLevelSpecification)
            {
                yield return viscosityLevelSpecification;
            }
        }
    }
}
