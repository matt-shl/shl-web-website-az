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
        if (block is null)
        {
            return null;
        }

        return new ProductDescription
        {
            Title = block.Title,
            Image = Image.Create(block.Image, cssClasses: "product-description__image"),
            SubTitle = block.SubTitle,
            Text = block.Text?.ToHtmlString(),
        };
    }
}
