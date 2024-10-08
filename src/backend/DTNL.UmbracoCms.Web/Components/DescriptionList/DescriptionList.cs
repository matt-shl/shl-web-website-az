using DTNL.UmbracoCms.Web.Helpers.Aliases;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public partial class DescriptionList
{
    public string? Title { get; set; }

    public required List<DescriptionListItem> Items { get; set; }

    public Button? DownloadLinkButton { get; set; }

    public static DescriptionList? Create(
        NestedBlockProductSpecifications? productSpecificationsBlock,
        PageProduct? productPage)
    {
        List<DescriptionListItem> items = new List<DescriptionListItem>();
        string title = "";

        if (productSpecificationsBlock is not null)
        {
            items = DescriptionListItem.CreateFor(productPage)
            .Concat(productSpecificationsBlock.Specifications
                .Using(s => s.Content as NestedBlockProductSpecification)
                .Using(DescriptionListItem.Create))
            .ToList();
            title = productSpecificationsBlock.Title!;
        }
        else
        {
            NestedBlockProductSpecifications? productSpecifications =
            productPage?.Specifications.GetSingleContentOrNull<NestedBlockProductSpecifications>();

            items = DescriptionListItem.CreateFor(productPage)
                .Concat(productSpecifications is not null ? productSpecifications.Specifications
                    .Using(s => s.Content as NestedBlockProductSpecification)
                    .Using(DescriptionListItem.Create) : [])
                .ToList();
            title = productSpecifications?.Title ?? "";
        }

        if (items.Count == 0)
        {
            return null;
        }

        return new DescriptionList
        {
            Title = title,
            Items = items,
            DownloadLinkButton = Button.Create(Link.Create(productPage?.SpecificationsFile))
                .With(b =>
                {
                    b.Class = "description-list__cta";
                    b.IconPosition = "start";
                    b.Icon = SvgAliases.Icons.ArrowTopRight;
                }),
        };
    }
}
