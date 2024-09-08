using DTNL.UmbracoCms.Web.Helpers.Aliases;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class EmptySection
{
    public required string Title { get; set; }

    public required string Text { get; set; }

    public Button? PrimaryLinkButton { get; set; }

    public Button? SecondaryLinkButton { get; set; }

    public static EmptySection Create(ICompositionNoResults noResults)
    {
        NestedBlockButtonLink? primaryLinkButtonContent = noResults.NoResultsPrimaryLink?.FirstOrDefault()?.Content as NestedBlockButtonLink;
        NestedBlockButtonLink? secondaryLinkButtonContent = noResults.NoResultsSecondaryLink?.FirstOrDefault()?.Content as NestedBlockButtonLink;

        return new EmptySection
        {
            Title = noResults.NoResultsTitle!,
            Text = noResults.NoResultsText!.ToHtmlString()!,
            PrimaryLinkButton = Button.Create(primaryLinkButtonContent)
                .With(b =>
                {
                    b.Variant = primaryLinkButtonContent?.Variant ?? "primary";
                    b.Icon = SvgAliases.Icons.ArrowTopRight;
                }),
            SecondaryLinkButton = Button.Create(secondaryLinkButtonContent)
                .With(b =>
                {
                    b.Variant = secondaryLinkButtonContent?.Variant ?? "secondary";
                    b.Icon = SvgAliases.Icons.ArrowTopRight;
                }),
        };
    }
}
