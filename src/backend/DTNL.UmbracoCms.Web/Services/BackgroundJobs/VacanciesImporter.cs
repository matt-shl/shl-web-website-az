using DTNL.UmbracoCms.Web.Helpers;
using DTNL.UmbracoCms.Web.Infrastructure.ApiClients;
using DTNL.UmbracoCms.Web.Modules.BackgroundJobs.Hangfire;
using Hangfire;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Scoping;

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
    private readonly ILogger<VacanciesImporter> _logger;

    public VacanciesImporter(
        IScopeProvider scopeProvider,
        IUmbracoContextFactory umbracoContextFactory,
        IVariationContextAccessor variationContextAccessor,
        IDefaultCultureAccessor defaultCultureAccessor,
        IServiceProvider serviceProvider,
        IContentService contentService,
        IAtsApiClient atsApiClient,
        ILogger<VacanciesImporter> logger)
    {
        _scopeProvider = scopeProvider;
        _umbracoContextFactory = umbracoContextFactory;
        _variationContextAccessor = variationContextAccessor;
        _defaultCultureAccessor = defaultCultureAccessor;

        _serviceProvider = serviceProvider;
        _contentService = contentService;
        _atsApiClient = atsApiClient;
        _logger = logger;
    }

    public async Task Run(IJobCancellationToken cancellationToken)
    {
        _logger.LogInformation("Vacancies importer started");

        using UmbracoContextReference umbracoContextReference = _umbracoContextFactory.EnsureUmbracoContext();
        using VariationContextHelper variationContextHelper = new(_variationContextAccessor, _defaultCultureAccessor.DefaultCulture);

        using IServiceScope serviceScope = _serviceProvider.CreateScope();
        IPublishedContentQuery publishedContentQuery = serviceScope.ServiceProvider.GetRequiredService<IPublishedContentQuery>();

        //IntradoNewsFolder? intradoNewsFolder = publishedContentQuery
        //    .ContentAtRoot()
        //    .OfType<PageHome>()
        //    .FirstOrDefault()?
        //    .FirstChild<IntradoNewsFolder>();

        //if (intradoNewsFolder == null)
        //{
        //    _logger.LogWarning("Importer finished - Vacancies Overview page could not be found.");
        //    return;
        //}

        //await ImportNews(intradoNewsFolder, cancellationToken.ShutdownToken);

        _logger.LogInformation("Vacancies importer finished");
    }

    //private async Task ImportNews(
    //    IntradoNewsFolder intradoNewsFolder, CancellationToken cancellationToken = default)
    //{
    //    NewsArticle[] newsArticles = await _intradoClient.GetAllNewsArticles(cancellationToken);

    //    foreach (NewsArticle newsArticle in newsArticles)
    //    {
    //        try
    //        {
    //            PageNewsIntrado? existingPageNewsIntrado = intradoNewsFolder
    //                .Children<PageNewsIntrado>()?
    //                .FirstOrDefault(p => p.ExternalId == newsArticle.Identifier);

    //            if (newsArticle.ModifiedDate.TruncateTo(DateTruncate.Second) <= (existingPageNewsIntrado?.LastUpdatedAt ?? DateTime.MinValue))
    //            {
    //                continue;
    //            }

    //            string nodeName = $"{newsArticle.Title} ({newsArticle.Identifier})";

    //            IContent pageNewsIntrado = existingPageNewsIntrado != null
    //                ? _contentService.GetById(existingPageNewsIntrado.Id) ??
    //                  throw new InvalidOperationException("Intrado news page node couldn't be found")
    //                : CreateIfNotExists(nodeName, intradoNewsFolder.Id, PageNewsIntrado.ModelTypeAlias);

    //            pageNewsIntrado.Name = nodeName;
    //            pageNewsIntrado.SetValue<PageNewsIntrado>(x => x.ExternalId, newsArticle.Identifier);
    //            pageNewsIntrado.SetValue<PageNewsIntrado>(x => x.Title, newsArticle.Title);
    //            pageNewsIntrado.SetValue<PageNewsIntrado>(x => x.Date, newsArticle.ReleaseDateTime);
    //            pageNewsIntrado.SetValue<PageNewsIntrado>(x => x.LastUpdatedAt, DateTime.UtcNow);
    //            pageNewsIntrado.SetValue<PageNewsIntrado>(x => x.CategoriesRaw, newsArticle.Category);

    //            SaveOrPublish(pageNewsIntrado);

    //            // Delay for 2 seconds to see if it solves the database timeout issue
    //            await Task.Delay(2000, cancellationToken);
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Unexpected error importing news article {NewsArticleIdentifier}, {ExMessage}", newsArticle.Identifier, ex.Message);
    //        }
    //    }
    //}

    private IContent CreateIfNotExists(string name, int parentId, string documentTypeAlias)
    {
        IContent? existingItem = _contentService.GetPagedChildren(parentId, 0, 1, out _, _scopeProvider.SqlContext.Query<IContent>().Where(c => c.Name == name)).FirstOrDefault();

        if (existingItem != null)
        {
            if (existingItem.ContentType.Alias != documentTypeAlias)
            {
                throw new InvalidOperationException($"Document with name '{name}' already exists with documentTypeAlias '{existingItem.ContentType.Alias}' instead of '{documentTypeAlias}'");
            }

            _logger.LogWarning("Document {Name} already existed but couldn't be found in the cache", name);
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
        }
        else
        {
            _ = _contentService.SaveAndPublish(content);
        }
    }
}
