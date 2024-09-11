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
        return new EmptySection
        {
            Title = noResults.NoResultsTitle!,
            Text = noResults.NoResultsText!.ToHtmlString()!,
            PrimaryLinkButton = Button.Create(noResults.NoResultsPrimaryLink, fallBackVariant: "primary"),
            SecondaryLinkButton = Button.Create(noResults.NoResultsSecondaryLink, fallBackVariant: "secondary"),
        };
    }

    public static EmptySection Create(PageError errorPage)
    {
        return new EmptySection
        {
            Title = errorPage.ErrorMessage!,
            Text = errorPage.ErrorDescription!.ToHtmlString()!,
            PrimaryLinkButton = Button.Create(errorPage.PrimaryLink, fallBackVariant: "primary"),
            SecondaryLinkButton = Button.Create(errorPage.SecondaryLink, fallBackVariant: "secondary"),
            Variant = "tight",
        };
    }
}
