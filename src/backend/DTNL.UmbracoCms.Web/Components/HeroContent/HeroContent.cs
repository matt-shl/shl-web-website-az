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

    public Button? FirstButton { get; set; }

    public Button? SecondButton { get; set; }

    public static HeroContent? Create(NestedBlockContentHero? contentHero, ICompositionBasePage page)
    {
        if (contentHero is null)
        {
            return null;
        }

        bool isFirstButtonPrimary = contentHero.PrimaryButton?.Equals("Second", StringComparison.OrdinalIgnoreCase) ?? true;

        return new HeroContent
        {
            ThemeCssClasses = ThemeHelper.GetCssClasses(page),
            Title = contentHero.Title,
            Subtitle = contentHero.Subtitle,
            Tags = contentHero.Tags?.Take(2).Select(tag => new Tag()
            {
                Label = tag,
                CssClasses = "hero-content__tag",

            }) ?? [],
            ShortDescription = contentHero.ShortDescription,
            FirstButton = Button.Create(contentHero.FirstLink)
                .With(b =>
                {
                    b.Class = "hero-content__cta";
                    b.Icon = SvgAliases.Icons.ArrowTopRight;
                    b.Variant = isFirstButtonPrimary ? null : "secondary";
                }),
            SecondButton = Button.Create(contentHero.SecondLink)
                .With(b =>
                {
                    b.Class = "hero-content__cta";
                    b.Icon = SvgAliases.Icons.ArrowTopRight;
                    b.Variant = !isFirstButtonPrimary ? null : "secondary";
                }),
        };
    }
}
