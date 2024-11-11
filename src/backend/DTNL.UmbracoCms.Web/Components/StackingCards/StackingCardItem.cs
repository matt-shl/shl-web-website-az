using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class StackingCardItem
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required Image Image { get; set; }

    public static StackingCardItem Create(NestedBlockStackingCard stackedCardBlock)
    {
        MediaWithCrops imageContent = stackedCardBlock.Image!;

        return new StackingCardItem
        {
            Title = stackedCardBlock.Title!,
            Description = stackedCardBlock.Description!,
            Image = Image.Create(imageContent, style: "stacking-card", objectFit: true)!,
        };
    }
}
