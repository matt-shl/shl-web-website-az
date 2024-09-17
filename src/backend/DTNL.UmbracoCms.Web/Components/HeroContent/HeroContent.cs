using DTNL.UmbracoCms.Web.Components.Hero;
using DTNL.UmbracoCms.Web.Helpers;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class HeroContent : IHero
{
    public string? ThemeCssClasses { get; set; }

    public string? Title { get; set; }

    public string? SubTitle { get; set; }

    public string? ShortDescription { get; set; }

    public IEnumerable<Tag>? Tags { get; set; }

    public Button? PrimaryButton { get; set; }

    public Button? SecondaryButton { get; set; }

    public bool ShowSearch { get; set; }

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

            SubTitle = contentHero.SubTitle.FallBack((page as ICompositionContentDetails)?.Date.ToLongDateString()),

            Tags = (page as ICompositionContentDetails)?.ContentTags?.Take(2).Select(tag => new Tag
            {
                Label = tag,
                CssClasses = "hero-content__tag",
            }) ?? [],

            ShortDescription = contentHero.Text?.ToHtmlString(),

            PrimaryButton = Button
                .Create(contentHero.PrimaryLink, fallBackVariant: "primary")
                .With(b =>
                {
                    b.Class = "hero-content__cta";
                    b.Hook = "homepage-hero-button";
                }),

            SecondaryButton = Button
                .Create(contentHero.SecondaryLink, fallBackVariant: "secondary")
                .With(b =>
                {
                    b.Class = "hero-content__cta";
                    b.Hook = "homepage-hero-button";
                }),
            ShowSearch = page is PageVacancyOverview,
        };
    }
}
