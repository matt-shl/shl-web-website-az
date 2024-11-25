using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class StackingCardItem
{
    public required string Title { get; set; }

    public required string Description { get; set; }

    public required Image Image { get; set; }

    public static StackingCardItem Create(NestedBlockStackingCard stackedCardBlock)
    {
        return new StackingCardItem
        {
            Title = stackedCardBlock.Title!,
            Description = stackedCardBlock.Description!,
            Image = Image.Create(stackedCardBlock.Image, style: "stacking-card")!,
        };
    }
}
