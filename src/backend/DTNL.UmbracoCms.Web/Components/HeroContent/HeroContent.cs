using DTNL.UmbracoCms.Web.Components.Hero;
using DTNL.UmbracoCms.Web.Helpers;
using DTNL.UmbracoCms.Web.Helpers.Aliases;

using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class HeroContent : IHero
{
    public string? ThemeCssClasses { get; set; }

    public string? Title { get; set; }

    public string? Subtitle { get; set; }

    public string? ShortDescription { get; set; }

    public IEnumerable<Tag>? Tags { get; set; }

    public ButtonLink? FirstButton { get; set; }

    public ButtonLink? SecondButton { get; set; }

    public static HeroContent? Create(NestedBlockContentHero? contentHero, ICompositionBasePage page)
    {
        if (contentHero is null)
        {
            return null;
        }

        return new HeroContent
        {
            ThemeCssClasses = contentHero.Theme is not null ? $"t-{contentHero.Theme?.Label ?? "general"}" : ThemeHelper.GetCssClasses(page),

            Title = contentHero.Title,

            Subtitle = contentHero.Subtitle,

            Tags = contentHero.Tags?.Take(2).Select(tag => new Tag()
            {
                Label = tag,
                CssClasses = "hero-content__tag",
            }) ?? [],

            ShortDescription = contentHero.ShortDescription,

            FirstButton = ButtonLink.Create(contentHero.PrimaryLink?.FirstOrDefault(), cssClasses: "hero-content__cta", jsHook: "homepage-hero-button", svgIcon: SvgAliases.Icons.ArrowTopRight),

            SecondButton = ButtonLink.Create(contentHero.SecondaryLink?.FirstOrDefault(), cssClasses: "hero-content__cta", jsHook: "homepage-hero-button", svgIcon: SvgAliases.Icons.ArrowTopRight),
        };
    }
}
