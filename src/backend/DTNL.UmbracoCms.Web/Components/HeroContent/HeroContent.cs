using DTNL.UmbracoCms.Web.Components.Hero;
using DTNL.UmbracoCms.Web.Helpers;
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

    public Button? PrimaryButton { get; set; }

    public Button? SecondaryButton { get; set; }

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

            Tags = contentHero.Tags?.Take(2).Select(tag => new Tag
            {
                Label = tag,
                CssClasses = "hero-content__tag",
            }) ?? [],

            ShortDescription = contentHero.ShortDescription,

            PrimaryButton = Button
                .Create(contentHero.PrimaryLink.GetSingleContentOrNull<NestedBlockButtonLink>())
                .With(b =>
                {
                    b.Class = "hero-content__cta";
                    b.Hook = "homepage-hero-button";
                }),

            SecondaryButton = Button
                .Create(contentHero.SecondaryLink.GetSingleContentOrNull<NestedBlockButtonLink>())
                .With(b =>
                {
                    b.Class = "hero-content__cta";
                    b.Hook = "homepage-hero-button";
                }),
        };
    }
}
