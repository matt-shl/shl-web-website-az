using DTNL.UmbracoCms.Web.Components.PartialComponent;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public interface ICard : IPartialViewPath
{
    private static readonly string[] ImageRatios = ["3-4", "4-3"];

    static ICard? Create(IPublishedElement card, string? cssClasses = null)
    {
        return card switch
        {
            NestedBlockImageCard imageCard => CardImage.Create(imageCard, cssClasses),
            NestedBlockNumberCard numberCard => CardNumber.Create(numberCard, cssClasses),
            NestedBlockIconCard iconCard => CardIcon.Create(iconCard, cssClasses),
            NestedBlockPageCard pageCard => CardKnowledge.Create(pageCard, cssClasses),
            NestedBlockContactCard contactCard => CardContact.Create(contactCard, cssClasses),
            NestedBlockProductCard { ProductPage: PageProduct productPage } => CardProduct.Create(productPage, cssClasses),
            NestedBlockImageCaptionCard imageCaptionCard => Image
                .Create(
                    imageCaptionCard.Image,
                    objectFit: false,
                    style: $"image{imageCaptionCard.SizeRatio.FallBack(ImageRatios.GetRandom())}",
                    cssClasses: cssClasses),
            _ => null,
        };
    }
}
