using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class ProductDescription
{
    public string? Title { get; set; }

    public Image? Image { get; set; }

    public string? SubTitle { get; set; }

    public string? Text { get; set; }

    public static ProductDescription? Create(NestedBlockProductBanner? block)
    {
        if (block is not NestedBlockProductBanner productBannerBlock)
        {
            return null;
        }

        return new ProductDescription
        {
            Title = productBannerBlock.Title,
            Image = Image.Create(productBannerBlock.Image, cssClasses: "product-description__image"),
            SubTitle = productBannerBlock.SubTitle,
            Text = productBannerBlock.Text?.ToHtmlString(),
        };
    }
}
