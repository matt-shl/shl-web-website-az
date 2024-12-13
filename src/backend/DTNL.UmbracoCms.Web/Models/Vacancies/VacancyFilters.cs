using DTNL.UmbracoCms.Web.Models.Filters;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Models.Vacancies;

public class VacancyFilters : BaseFilters
{
    public static readonly (string Name, Func<PageVacancy, IEnumerable<string>?> GetValues)[] QuickFilterFields =
    [
        (nameof(PageVacancy.Function), p => p.Function.IsNullOrWhiteSpace() ? null : [p.Function]),
        (nameof(PageVacancy.Country), p => p.Country.IsNullOrWhiteSpace() ? null : [p.Country]),
        (nameof(PageVacancy.JobLevel), p => p.JobLevel.IsNullOrWhiteSpace() ? null : [p.JobLevel]),
    ];

    public static readonly (string Name, Func<PageVacancy, IEnumerable<string>?> GetValues)[] FilterFields =
    [
        ..QuickFilterFields,
        (nameof(PageVacancy.ContractType), p => p.ContractType.IsNullOrWhiteSpace() ? null : [p.ContractType]),
    ];

    public VacancyFilters(PageVacancyOverview overviewPage, IQueryCollection queryCollection)
        : base(overviewPage, queryCollection)
    {
    }
}
