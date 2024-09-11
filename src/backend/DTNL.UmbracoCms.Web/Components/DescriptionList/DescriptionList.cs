using DTNL.UmbracoCms.Web.Helpers.Aliases;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public partial class DescriptionList
{
    public string? Title { get; set; }

    public required List<DescriptionListItem> Items { get; set; }

    public Button? DownloadLinkButton { get; set; }


    public static DescriptionList? Create(PageProduct productPage)
    {
        return Create(
            productPage.Specifications.GetSingleContentOrNull<NestedBlockProductSpecifications>(),
            productPage);
    }

    public static DescriptionList? Create(
        NestedBlockProductSpecifications? productSpecificationsBlock,
        PageProduct? productPage = null)
    {
        if (productSpecificationsBlock is null)
        {
            return null;
        }

        List<DescriptionListItem> items =
            DescriptionListItem.CreateFor(productPage)
            .Concat(productSpecificationsBlock.Specifications
                .Using(s => s.Content as NestedBlockProductSpecification)
                .Using(DescriptionListItem.Create))
            .ToList();

        if (items.Count == 0)
        {
            return null;
        }

        return new DescriptionList
        {
            Title = productSpecificationsBlock.Title,
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
