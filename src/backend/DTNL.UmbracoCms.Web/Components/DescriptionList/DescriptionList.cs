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
        PageProduct? productPage,
        NestedBlockProductSpecifications? productSpecificationsBlock = null)
    {
        productSpecificationsBlock ??= productPage?.Specifications.GetSingleContentOrNull<NestedBlockProductSpecifications>();

        List<DescriptionListItem> items = [];
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
