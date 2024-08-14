using DTNL.UmbracoCms.Web.Components.Hero;
using DTNL.UmbracoCms.Web.Helpers;
using DTNL.UmbracoCms.Web.Helpers.Aliases;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class HeroContent : IHero
{
    public string? ThemeCssClasses { get; set; }

    public string? Title { get; set; }

    public string? Subtitle { get; set; }

    public string? ShortDescription { get; set; }

    public IEnumerable<Tag>? Tags { get; set; }

    public required Button? PrimaryLinkButton { get; set; }

    public required Button? SecondaryLinkButton { get; set; }


    public static HeroContent? Create(NestedBlockContentHero? contentHero, ICompositionBasePage page)
    {
        if (contentHero is null)
        {
            return null;
        }

        return new HeroContent
        {
            ThemeCssClasses = ThemeHelper.GetCssClasses(page),
            Title = contentHero.Title,
            Subtitle = contentHero.Subtitle,
            Tags = contentHero.Tags?.Select(tag => new Tag()
            {
                Label = tag,
                CssClasses = "hero-content__tag",

            }) ?? [],
            ShortDescription = contentHero.ShortDescription,
            PrimaryLinkButton = Button.Create(contentHero.PrimaryLink)
                .With(b =>
                {
                    b.Class = "hero-content__cta";
                    b.Icon = SvgAliases.Icons.ArrowTopRight;
                }),
            SecondaryLinkButton = Button.Create(contentHero.SecondaryLink)
                .With(b =>
                {
                    b.Class = "hero-content__cta";
                    b.Icon = SvgAliases.Icons.ArrowTopRight;
                    b.Variant = "secondary";
                }),
        };
    }
}
