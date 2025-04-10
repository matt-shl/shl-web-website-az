using DTNL.UmbracoCms.Web.Helpers;
using DTNL.UmbracoCms.Web.Helpers.Aliases;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using DTNL.UmbracoCms.Web.Models.Filters;
using DTNL.UmbracoCms.Web.Models.Vacancies;
using DTNL.UmbracoCms.Web.Services;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class OverviewVacancies : OverviewFor<PageVacancyOverview, PageVacancy, VacancyFilters, JobListingItem>
{
    public OverviewVacancies(ISearchService searchService)
        : base(searchService)
    {
    }

    public override LayoutSection LayoutSection => new()
    {
        CssThemeClasses = ThemeHelper.GetCssClasses(OverviewPage.VacanciesOverviewTheme),
        Variant = "grid-single-column",
        ReduceMargin = "top-bottom",
        Id = "content",
        ListLabel = CultureDictionary.GetTranslation(TranslationAliases.Vacancies),
    };

    protected override PageVacancyOverview OverviewPage => NodeProvider.VacancyOverviewPage!;

    protected override List<PageVacancy> GetPages()
    {
        return OverviewPage
            .Children<PageVacancy>()
            .OrEmptyIfNull()
            .ToList();
    }

    protected override VacancyFilters ApplyFilters(List<PageVacancy> pages)
    {
        VacancyFilters vacancyFilters = new(OverviewPage, Request.Query);

        vacancyFilters.AddFilterOptions(VacancyFilters.FilterFields, pages, HttpContext);

        foreach ((string name, Func<PageVacancy, IEnumerable<string>?> getValues)
                 in VacancyFilters.FilterFields)
        {
            if (!vacancyFilters.TryGetValue(name, out FilterOption[]? filterOptions) ||
                !FilterOption.AnySelected(filterOptions))
            {
                continue;
            }

            pages.RemoveAll(page => !FilterOption.AnySelectedValueIn(filterOptions, getValues(page)));
        }

        GetAndApplySorting(vacancyFilters, pages);

        return vacancyFilters;
    }

    protected override Filters? GetFilters(VacancyFilters? filters, List<PageVacancy> pages)
    {
        return filters is null ? null : Filters.Create(filters, TotalCount, pages, CultureDictionary);
    }

    protected override IEnumerable<JobListingItem> MapToOverviewItems(List<PageVacancy> pages)
    {
        return pages.Using(JobListingItem.Create);
    }
}
