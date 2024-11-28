using System.Globalization;
using DTNL.UmbracoCms.Web.Helpers;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using DTNL.UmbracoCms.Web.Infrastructure.ApiClients.Ats;
using DTNL.UmbracoCms.Web.Infrastructure.ApiClients.Ats.Models;
using DTNL.UmbracoCms.Web.Modules.BackgroundJobs.Hangfire;
using Hangfire;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Services.BackgroundJobs;

public class VacanciesImporter : IBackgroundJob
{
    private readonly IScopeProvider _scopeProvider;
    private readonly IUmbracoContextFactory _umbracoContextFactory;
    private readonly IVariationContextAccessor _variationContextAccessor;
    private readonly IDefaultCultureAccessor _defaultCultureAccessor;
    private readonly IServiceProvider _serviceProvider;
    private readonly IContentService _contentService;
    private readonly IAtsApiClient _atsApiClient;
    private readonly VacanciesContentHelper _vacanciesContentHelper;
    private readonly ILogger<VacanciesImporter> _logger;

    public VacanciesImporter(
        IScopeProvider scopeProvider,
        IUmbracoContextFactory umbracoContextFactory,
        IVariationContextAccessor variationContextAccessor,
        IDefaultCultureAccessor defaultCultureAccessor,
        IServiceProvider serviceProvider,
        IContentService contentService,
        IAtsApiClient atsApiClient,
        VacanciesContentHelper vacanciesContentHelper,
        ILogger<VacanciesImporter> logger)
    {
        _scopeProvider = scopeProvider;
        _umbracoContextFactory = umbracoContextFactory;
        _variationContextAccessor = variationContextAccessor;
        _defaultCultureAccessor = defaultCultureAccessor;

        _serviceProvider = serviceProvider;
        _contentService = contentService;
        _atsApiClient = atsApiClient;
        _vacanciesContentHelper = vacanciesContentHelper;
        _logger = logger;
    }

    public async Task Run(IJobCancellationToken cancellationToken)
    {
        _logger.LogInformation("Vacancies importer started");

        CultureInfo defaultCulture = new(_defaultCultureAccessor.DefaultCulture);
        Thread.CurrentThread.CurrentCulture = defaultCulture;
        Thread.CurrentThread.CurrentUICulture = defaultCulture;

        using UmbracoContextReference umbracoContextReference = _umbracoContextFactory.EnsureUmbracoContext();
        using VariationContextHelper variationContextHelper = new(_variationContextAccessor, _defaultCultureAccessor.DefaultCulture);

        using IServiceScope serviceScope = _serviceProvider.CreateScope();
        IPublishedContentQuery publishedContentQuery = serviceScope.ServiceProvider.GetRequiredService<IPublishedContentQuery>();

        PageHome? homePage = publishedContentQuery
            .ContentAtRoot()
            .OfType<PageHome>()
            .FirstOrDefault();

        PageVacancyOverview? vacancyOverviewPage = NodeProvider.GetVacancyOverviewPage(homePage);

        if (vacancyOverviewPage is null)
        {
            _logger.LogWarning("Importer finished - Vacancies Overview page could not be found.");
            return;
        }

        List<AtsVacancy>? vacancies = await _atsApiClient.GetAllVacancies(cancellationToken.ShutdownToken);

        if (vacancies is null)
        {
            _logger.LogWarning("Importer finished - Vacancies could not be retrieved.");
            return;
        }

        RemoveVacancies(vacancyOverviewPage, vacancies, cancellationToken.ShutdownToken);

        AddOrUpdateVacancies(vacancyOverviewPage, vacancies, cancellationToken.ShutdownToken);

        _logger.LogInformation("Vacancies importer finished");
    }

    private void AddOrUpdateVacancies(
        PageVacancyOverview vacancyOverviewPage,
        List<AtsVacancy> vacancies,
        CancellationToken cancellationToken = default)
    {
        string[] vacancyCultures = vacancyOverviewPage.Cultures.Keys.ToArray();

        foreach (AtsVacancy vacancy in vacancies)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            try
            {
                PageVacancy? existingPageVacancy = vacancyOverviewPage
                    .Children<PageVacancy>()?
                    .FirstOrDefault(pageVacancy => pageVacancy.ExternalId == vacancy.Id);

                string nodeName = $"{vacancy.Title ?? vacancy.Id}";

                IContent? pageVacancyContent = existingPageVacancy is null
                    ? CreateIfNotExists(nodeName, vacancyOverviewPage.Id, PageVacancy.ModelTypeAlias)
                    : _contentService.GetById(existingPageVacancy.Key);

                if (pageVacancyContent is null)
                {
                    _logger.LogWarning("Could not import content for vacancy {ID}", vacancy.Id);
                    continue;
                }

                _vacanciesContentHelper.SetVacancyContent(pageVacancyContent, vacancy, vacancyCultures);

                SaveOrPublish(pageVacancyContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error importing vacancy {ID}, {ExceptionMessage}", vacancy.Id, ex.Message);
            }
        }
    }

    private void RemoveVacancies(
        PageVacancyOverview vacancyOverviewPage,
        List<AtsVacancy> vacancies,
        CancellationToken cancellationToken = default)
    {
        HashSet<string?> vacancyIds = vacancies.Select(vacancy => vacancy.Id).ToHashSet();

        List<IContent> vacancyPagesToRemove = _contentService
            .GetPagedChildren(vacancyOverviewPage.Id, 0, int.MaxValue, out _)
            .Where(child => child.ContentType.Alias == PageVacancy.ModelTypeAlias)
            .Where(pageVacancy => !vacancyIds.Contains(pageVacancy.GetValue<PageVacancy, string?>(p => p.ExternalId)))
            .ToList();

        foreach (IContent vacancyPage in vacancyPagesToRemove)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            if (!vacancyPage.Published)
            {
                if ((DateTime.UtcNow - vacancyPage.GetValue<PageVacancy, DateTime>(p => p.LastUpdatedAt)).Days > 30)
                {
                    DeleteItem(vacancyPage);
                }
            }
            else
            {
                UnpublishItem(vacancyPage);
            }
        }
    }

    private IContent CreateIfNotExists(string name, int parentId, string documentTypeAlias)
    {
        IContent? existingItem = _contentService
            .GetPagedChildren(parentId, 0, 1, out _, _scopeProvider.SqlContext.Query<IContent>().Where(c => c.Name == name))
            .FirstOrDefault();

        if (existingItem != null)
        {
            if (existingItem.ContentType.Alias != documentTypeAlias)
            {
                throw new InvalidOperationException($"Document with name '{name}' already exists with documentTypeAlias '{existingItem.ContentType.Alias}' instead of '{documentTypeAlias}'");
            }

            _logger.LogWarning("Vacancy {Name} already existed but couldn't be found in the cache", name);
        }

        return existingItem ?? _contentService.Create(name, parentId, documentTypeAlias);
    }

    private void SaveOrPublish(IContent content)
    {
        if (!content.IsDirty())
        {
            return;
        }

        if (content.Edited)
        {
            _ = _contentService.Save(content);

            _logger.LogWarning("Vacancy {Name} saved", content.Name);
        }
        else
        {
            _ = _contentService.SaveAndPublish(content);

            _logger.LogWarning("Vacancy {Name} published", content.Name);
        }
    }

    private void DeleteItem(IContent existingItem)
    {
        OperationResult deleteResult = _contentService.Delete(existingItem);

        if (deleteResult.Success)
        {
            _logger.LogInformation("Vacancy {Key} removed", existingItem.Key);
        }
        else
        {
            _logger
                .LogWarning(
                    "Vacancy {Key} could not be removed: {ErrorMessages}",
                    existingItem.Key,
                    string.Join(',', deleteResult.EventMessages?.GetAll().Select(m => m.Message) ?? []));
        }
    }

    private void UnpublishItem(IContent existingItem)
    {
        PublishResult publishResult = _contentService.Unpublish(existingItem);

        if (publishResult.Success)
        {
            _logger.LogInformation("Vacancy {Key} unpublished", existingItem.Key);
        }
        else
        {
            _logger
                .LogWarning(
                    "Vacancy {Key} could not be unpublished: {ErrorMessages}",
                    existingItem.Key,
                    string.Join(',', publishResult.EventMessages?.GetAll().Select(m => m.Message) ?? []));
        }
    }
}
