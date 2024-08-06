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
        return new EmptySection
        {
            Title = noResults.NoResultsTitle!,
            Text = noResults.NoResultsText!.ToHtmlString()!,
            PrimaryLinkButton = Button.Create(noResults.PrimaryLink)
                .With(b =>
                {
                    b.Variant = "primary";
                    b.Icon = SvgAliases.Icons.ArrowTopRight;
                }),
            SecondaryLinkButton = Button.Create(noResults.SecondaryLink)
                .With(b =>
                {
                    b.Variant = "secondary";
                    b.Icon = SvgAliases.Icons.ArrowTopRight;
                }),
        };
    }
}
