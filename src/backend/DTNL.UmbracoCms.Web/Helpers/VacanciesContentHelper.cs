using System.Globalization;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using DTNL.UmbracoCms.Web.Infrastructure.ApiClients.Ats.Models;
using DTNL.UmbracoCms.Web.Infrastructure.DependencyInjection;
using Nager.Country.Translation;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Helpers;

[Transient]
public class VacanciesContentHelper
{
    private static readonly string[] VacanciesContentTypeAliases = [NestedBlockContentHero.ModelTypeAlias];

    private readonly List<IContentType> _vacanciesContentTypes;

    public VacanciesContentHelper(IContentTypeService contentTypeService)
    {
        _vacanciesContentTypes = contentTypeService
            .GetAll()
            .Where(c => VacanciesContentTypeAliases.Contains(c.Alias))
            .ToList();
    }

    public void SetVacancyContent(IContent pageVacancyContent, AtsVacancy vacancy, IEnumerable<string> cultures)
    {
        foreach (string culture in cultures)
        {
            SetVacancyContent(pageVacancyContent, vacancy, culture);
        }
    }

    public void SetVacancyContent(
        IContent pageVacancyContent,
        AtsVacancy vacancy,
        string culture)
    {
        pageVacancyContent.SetCultureName(pageVacancyContent.Name, culture);

        pageVacancyContent.SetValue<PageVacancy>(p => p.ExternalId, vacancy.Id, culture);
        pageVacancyContent.SetValue<PageVacancy>(p => p.ExternalUrl, vacancy.Url, culture);
        pageVacancyContent.SetValue<PageVacancy>(p => p.CreatedAt, GetDateOrNull(vacancy.CreatedAt), culture);
        pageVacancyContent.SetValue<PageVacancy>(p => p.LastUpdatedAt, DateTime.UtcNow, culture);

        pageVacancyContent.SetValue<PageVacancy>(x => x.Title, vacancy.Title, culture);
        pageVacancyContent.SetValue<PageVacancy>(x => x.Company, vacancy.Facility, culture);
        pageVacancyContent.SetValue<PageVacancy>(x => x.Function, vacancy.Department, culture);
        pageVacancyContent.SetValue<PageVacancy>(x => x.ContractType, vacancy.ShiftType, culture);
        pageVacancyContent.SetValue<PageVacancy>(x => x.JobLevel, vacancy.CustomField1, culture);
        pageVacancyContent.SetValue<PageVacancy>(x => x.JobDescription, vacancy.Description, culture);

        pageVacancyContent.SetValue<PageVacancy>(x => x.City, vacancy.City, culture);
        pageVacancyContent.SetValue<PageVacancy>(x => x.State, vacancy.State, culture);
        pageVacancyContent.SetValue<PageVacancy>(x => x.PostalCode, vacancy.PostalCode, culture);
        pageVacancyContent.SetValue<PageVacancy>(x => x.Country, GetCountryName(vacancy.Country, culture), culture);

        SetHeroContent(pageVacancyContent, vacancy, culture);
    }

    private void SetHeroContent(IContent pageVacancyContent, AtsVacancy vacancy, string culture)
    {
        var blockContentHero = new
        {
            title = vacancy.Title,
            subTitle = $"{vacancy.City}, {GetCountryName(vacancy.Country, culture)}".TrimStart(", "),
        };

        string heroJson = BlockListCreatorService
            .GetBlockListJsonFor([blockContentHero], _vacanciesContentTypes.First(c => c.Alias == NestedBlockContentHero.ModelTypeAlias).Key);

        pageVacancyContent.SetValue<PageVacancy>(x => x.Hero, heroJson, culture);
    }

    private static DateTime? GetDateOrNull(string? dateString)
    {
        const string dateFormat = "ddd, dd MM yyyy HH:mm:ss 'GMT'";

        if (DateTime
            .TryParseExact(
                dateString,
                dateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal,
                out DateTime date))
        {
            return date;
        }

        return null;
    }

    public static string? GetCountryName(string? countryCode, string culture)
    {
        if (countryCode.IsNullOrWhiteSpace())
        {
            return null;
        }

        TranslationProvider translationProvider = new();

        string? countryName = translationProvider.GetCountryTranslatedName(countryCode, culture.Split("-").First());

        return countryName ?? countryCode;
    }
}
