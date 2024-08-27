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

        NestedBlockButtonLink? primaryButtonLink = contentHero.PrimaryLink?.FirstOrDefault()?.Content as NestedBlockButtonLink;
        NestedBlockButtonLink? secondaryButtonLink = contentHero.SecondaryLink?.FirstOrDefault()?.Content as NestedBlockButtonLink;

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

            FirstButton = Button.Create(primaryButtonLink?.Link).With(b =>
            {
                b.Class = "hero-content__cta";
                b.Hook = "homepage-hero-button";
                b.Icon = primaryButtonLink?.ButtonIcon?.LocalCrops.Src ?? SvgAliases.Icons.ArrowTopRight;
                b.Variant = primaryButtonLink?.Variant;
            }),

            SecondButton = Button.Create(secondaryButtonLink?.Link).With(b =>
            {
                b.Class = "hero-content__cta";
                b.Hook = "homepage-hero-button";
                b.Icon = secondaryButtonLink?.ButtonIcon?.LocalCrops.Src ?? SvgAliases.Icons.ArrowTopRight;
                b.Variant = secondaryButtonLink?.Variant;
            }),
        };
    }
}
