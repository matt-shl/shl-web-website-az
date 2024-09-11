using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class EmptySection
{
    public required string Title { get; set; }

    public required string Text { get; set; }

    public Button? PrimaryLinkButton { get; set; }

    public Button? SecondaryLinkButton { get; set; }

    public string? Variant { get; set; }

    public static EmptySection Create(ICompositionNoResults noResults)
    {
        NestedBlockButtonLink? primaryLinkButtonContent = noResults.NoResultsPrimaryLink.GetSingleContentOrNull<NestedBlockButtonLink>();
        NestedBlockButtonLink? secondaryLinkButtonContent = noResults.NoResultsSecondaryLink.GetSingleContentOrNull<NestedBlockButtonLink>();

        return new EmptySection
        {
            Title = noResults.NoResultsTitle!,
            Text = noResults.NoResultsText!.ToHtmlString()!,
            PrimaryLinkButton = Button.Create(primaryLinkButtonContent),
            SecondaryLinkButton = Button.Create(secondaryLinkButtonContent)
                .With(b =>
                {
                    b.Variant = secondaryLinkButtonContent?.Variant ?? "secondary";
                }),
            PrimaryLinkButton = Button.Create(primaryLinkButtonContent),
            SecondaryLinkButton = Button.Create(secondaryLinkButtonContent),
        };
    }

    public static EmptySection Create(PageError errorPage)
    {
        return new EmptySection
        {
            Title = errorPage.ErrorMessage!,
            Text = errorPage.ErrorDescription!.ToHtmlString()!,
            PrimaryLinkButton = Button.Create(primaryLinkButtonContent),
            SecondaryLinkButton = Button.Create(secondaryLinkButtonContent)
                .With(b =>
                    {
                        b.Variant = secondaryLinkButtonContent?.Variant ?? "secondary";
                    }),
            Variant = "tight",
        };
    }
}
