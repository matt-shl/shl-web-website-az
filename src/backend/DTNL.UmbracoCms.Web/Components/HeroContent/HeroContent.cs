using DTNL.UmbracoCms.Web.Components.Hero;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class HeroContent : IHero
{
    public string? ThemeCssClasses { get; set; }

    public required string Title { get; set; }

    public string? SubTitle { get; set; }

    public string? ShortDescription { get; set; }

    public required List<Tag> Tags { get; set; }

    public Button? PrimaryButton { get; set; }

    public Button? SecondaryButton { get; set; }

    public bool ShowSearch { get; set; }

    public static HeroContent? Create(NestedBlockContentHero? contentHero, ICompositionHero page)
    {
        if (contentHero is null)
        {
            return null;
        }

        return new HeroContent
        {
            Title = contentHero.Title!,
            SubTitle = contentHero.SubTitle.FallBack((page as ICompositionContentDetails)?.GetDate()?.ToString("MMMM dd yyyy")),
            Tags = GetTagValues(page)
                .Using(tag => Tag.Create(tag, "hero-content__tag"))
                .Take(2)
                .ToList(),
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
            ShowSearch = page is PageVacancyOverview or PageCareerOverview or PageSearch,
        };
    }

    public static IEnumerable<string?>? GetTagValues(ICompositionBasePage page)
    {
        return page switch
        {
            ICompositionContentDetails contentDetails => contentDetails.ContentTags,
            PageVacancy vacancyPage => [vacancyPage.ContractType, vacancyPage.JobLevel],
            _ => [],
        };
    }
}
