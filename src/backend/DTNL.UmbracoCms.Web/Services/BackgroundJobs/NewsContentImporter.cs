using System.Globalization;
using System.Xml.Linq;
using DTNL.UmbracoCms.Web.Helpers;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
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

public class NewsContentImporter : IBackgroundJob
{
    private readonly (string ContentFilePath, string ContentCulture)[] _contentFilePathsAndCultures =
    [
        (@".\\Files\\english-posts-shlmedical.WordPress.2024-11-26.xml", "en-US"),
        (@".\\Files\\chinese-posts-shlmedical.WordPress.2024-11-26.xml", "zh-Hant-TW"),
    ];

    private readonly IScopeProvider _scopeProvider;
    private readonly IUmbracoContextFactory _umbracoContextFactory;
    private readonly IVariationContextAccessor _variationContextAccessor;
    private readonly IDefaultCultureAccessor _defaultCultureAccessor;
    private readonly IServiceProvider _serviceProvider;
    private readonly IContentService _contentService;
    private readonly NewsContentHelper _newsContentHelper;
    private readonly ILogger<NewsContentImporter> _logger;

    public NewsContentImporter(
        IScopeProvider scopeProvider,
        IUmbracoContextFactory umbracoContextFactory,
        IVariationContextAccessor variationContextAccessor,
        IDefaultCultureAccessor defaultCultureAccessor,
        IServiceProvider serviceProvider,
        IContentService contentService,
        NewsContentHelper newsContentHelper,
        ILogger<NewsContentImporter> logger)
    {
        _scopeProvider = scopeProvider;
        _umbracoContextFactory = umbracoContextFactory;
        _variationContextAccessor = variationContextAccessor;
        _defaultCultureAccessor = defaultCultureAccessor;
        _serviceProvider = serviceProvider;
        _contentService = contentService;
        _newsContentHelper = newsContentHelper;
        _logger = logger;
    }

    public async Task Run(IJobCancellationToken cancellationToken)
    {
        _logger.LogInformation("Content importer started");

        foreach ((string contentFilePath, string contentCulture) in _contentFilePathsAndCultures)
        {
            CultureInfo culture = new(contentCulture);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            using UmbracoContextReference umbracoContextReference = _umbracoContextFactory.EnsureUmbracoContext();
            using VariationContextHelper variationContextHelper = new(_variationContextAccessor, _defaultCultureAccessor.DefaultCulture);

            using IServiceScope serviceScope = _serviceProvider.CreateScope();
            IPublishedContentQuery publishedContentQuery = serviceScope.ServiceProvider.GetRequiredService<IPublishedContentQuery>();

            PageOverview? overviewPage = publishedContentQuery
                .ContentAtRoot()
                .OfType<PageHome>()
                .FirstOrDefault()?
                .FirstChild<PageOverview>();

            if (overviewPage is null)
            {
                _logger.LogWarning("Importer finished - Overview page could not be found.");
                return;
            }

            AddOrUpdateContent(overviewPage, contentFilePath, [contentCulture], cancellationToken.ShutdownToken);
        }

        _logger.LogInformation("Vacancies importer finished");
    }

    private void AddOrUpdateContent(
        PageOverview overviewPage,
        string contentFilePath,
        string[] cultures,
        CancellationToken cancellationToken = default)
    {
        XDocument xmlDocument = XDocument.Load(contentFilePath);

        foreach (XElement post in xmlDocument.Descendants("item"))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            string? nodeName = ((string?) post.Element("title"))?.RemoveDiacritics();

            if (nodeName.IsNullOrWhiteSpace())
            {
                continue;
            }

            try
            {
                PageNews? existingPage = overviewPage
                    .Children<PageNews>()?
                    .FirstOrDefault(pageVacancy => pageVacancy.Name == nodeName);

                IContent? page = existingPage is null
                    ? CreateIfNotExists(nodeName, overviewPage.Id, PageNews.ModelTypeAlias)
                    : _contentService.GetById(existingPage.Key);

                if (page is null)
                {
                    _logger.LogWarning("Could not import content {ID}", nodeName);
                    continue;
                }

                _newsContentHelper.SetContent(page, post, cultures);

                SaveOrPublish(page);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error importing content {ID}, {ExceptionMessage}", nodeName, ex.Message);
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

            _logger.LogWarning("Content {Name} already existed but couldn't be found in the cache", name);
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

            _logger.LogWarning("Content {Name} saved", content.Name);
        }
        else
        {
            _ = _contentService.SaveAndPublish(content);

            _logger.LogWarning("Content {Name} published", content.Name);
        }
    }
}
