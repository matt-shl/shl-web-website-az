using DTNL.UmbracoCms.Web.Helpers.Aliases;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class JobVacancies : ViewComponentExtended
{
    public const int NumberOfVacancies = 3;

    public required string Title { get; set; }

    public required List<JobListingItem> Vacancies { get; set; }

    public Button? PrimaryButton { get; set; }

    public Button? SecondaryButton { get; set; }

    public static JobVacancies Create(NestedBlockVacancies vacanciesBlock, PageVacancyOverview vacancyOverviewPage)
    {
        List<PageVacancy> vacancyPages = vacanciesBlock.Vacancies
            .OrEmptyIfNull()
            .OfType<PageVacancy>()
            .ToList();

        if (vacancyPages.Count == 0)
        {
            vacancyPages.AddRange(Services.NodeProvider
                .GetVacancyPages(vacancyOverviewPage)
                .OrderByDescending(vacancyPage => vacancyPage.CreatedAt)
                .Take(NumberOfVacancies));
        }

        return new()
        {
            Title = vacanciesBlock.Title!,
            Vacancies = vacancyPages
                .Using(JobListingItem.Create)
                .ToList(),
            PrimaryButton = Button.Create(vacanciesBlock.PrimaryLink, fallBackVariant: "primary"),
            SecondaryButton = Button.Create(vacanciesBlock.SecondaryLink, fallBackVariant: "secondary"),
        };
    }

    public IViewComponentResult Invoke(PageVacancy page)
    {
        Title = CultureDictionary.GetTranslation(TranslationAliases.Vacancies.RelatedVacancies);

        Vacancies = page
            .Siblings<PageVacancy>()
            .OrEmptyIfNull()
            .Where(vacancyPage => vacancyPage.Function == page.Function)
            .Take(NumberOfVacancies)
            .Using(JobListingItem.Create)
            .ToList();

        PrimaryButton = new Button
        {
            Label = CultureDictionary.GetTranslation(TranslationAliases.Vacancies.AllVacancies),
            Url = NodeProvider.VacancyOverviewPage!.Url(),
            Variant = "primary",
            Icon = SvgAliases.Icons.ArrowTopRight,
        };

        if (!page.ExternalUrl.IsNullOrEmpty())
        {
            SecondaryButton = new Button
            {
                Label = CultureDictionary.GetTranslation(TranslationAliases.Vacancies.Apply),
                Url = page.ExternalUrl,
                Variant = "secondary",
                Icon = SvgAliases.Icons.ArrowTopRight,
            };
        }

        return View("JobVacancies", this);
    }
}
