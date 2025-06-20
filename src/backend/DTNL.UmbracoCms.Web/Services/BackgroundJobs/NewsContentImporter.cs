using System.Globalization;
using System.Xml.Linq;
using DTNL.UmbracoCms.Web.Helpers;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using DTNL.UmbracoCms.Web.Modules.BackgroundJobs.Hangfire;
using Hangfire;
using Microsoft.Extensions.FileProviders;
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
        ("Files/english-posts-shlmedical.WordPress.2024-11-26.xml", "en-US"),
        ("Files/chinese-posts-shlmedical.WordPress.2024-11-26.xml", "zh-Hant-TW"),
    ];

    private readonly IScopeProvider _scopeProvider;
    private readonly IUmbracoContextFactory _umbracoContextFactory;
    private readonly IVariationContextAccessor _variationContextAccessor;
    private readonly IDefaultCultureAccessor _defaultCultureAccessor;
    private readonly IServiceProvider _serviceProvider;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IContentService _contentService;
    private readonly NewsContentHelper _newsContentHelper;
    private readonly EventsContentHelper _eventsContentHelper;
    private readonly PubliationsContentHelper _publicationsContentHelper;
    private readonly ILogger<NewsContentImporter> _logger;

    public NewsContentImporter(
        IScopeProvider scopeProvider,
        IUmbracoContextFactory umbracoContextFactory,
        IVariationContextAccessor variationContextAccessor,
        IDefaultCultureAccessor defaultCultureAccessor,
        IServiceProvider serviceProvider,
        IWebHostEnvironment webHostEnvironment,
        IContentService contentService,
        NewsContentHelper newsContentHelper,
        EventsContentHelper eventsContentHelper,
        PubliationsContentHelper publiationsContentHelper,
        ILogger<NewsContentImporter> logger)
    {
        _scopeProvider = scopeProvider;
        _umbracoContextFactory = umbracoContextFactory;
        _variationContextAccessor = variationContextAccessor;
        _defaultCultureAccessor = defaultCultureAccessor;
        _serviceProvider = serviceProvider;
        _webHostEnvironment = webHostEnvironment;
        _contentService = contentService;
        _newsContentHelper = newsContentHelper;
        _eventsContentHelper = eventsContentHelper;
        _publicationsContentHelper = publiationsContentHelper;
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

            PageContent? mainContentPage = publishedContentQuery
                .ContentAtRoot()
                .OfType<PageHome>()
                .FirstOrDefault()?
                .Children()
                .OfType<PageContent>()
                .FirstOrDefault(p => p.GetTitle() is "News and insights");

            if (mainContentPage is null)
            {
                _logger.LogWarning("Importer finished - Main content page (News and insights) could not be found.");
                return;
            }

            PageOverview? eventsOverview = mainContentPage
                .Children()
                .OfType<PageOverview>()
                .FirstOrDefault(p => p.GetTitle() == "Events");

            PageOverview? publicationsOverview = mainContentPage
                .Children()
                .OfType<PageOverview>()
                .FirstOrDefault(p => p.GetTitle() == "Publications and press releases");

            PageOverview? newsOverview = mainContentPage
                .Children()
                .OfType<PageOverview>()
                .FirstOrDefault(p => p.GetTitle() == "News");

            if (newsOverview is null || publicationsOverview is null || eventsOverview is null)
            {
                _logger.LogWarning("Importer finished - Overview pages could not be found.");
                return;
            }

            await AddOrUpdateContent(eventsOverview, newsOverview, publicationsOverview, contentFilePath, [contentCulture], cancellationToken.ShutdownToken);
        }

        _logger.LogInformation("Vacancies importer finished");
    }

    private async Task AddOrUpdateContent(
        PageOverview eventsOverview,
        PageOverview newsOverview,
        PageOverview publicationsOverview,
        string contentFilePath,
        string[] cultures,
        CancellationToken cancellationToken = default)
    {
        IFileInfo fileInfo = _webHostEnvironment.ContentRootFileProvider.GetFileInfo(contentFilePath);
        if (!fileInfo.Exists)
        {
            _logger.LogWarning("Could not find {FilePath}", Path.Combine(_webHostEnvironment.ContentRootPath, contentFilePath));
            return;
        }

        await using Stream stream = fileInfo.CreateReadStream();

        XDocument xmlDocument = XDocument.Load(stream);

        foreach (XElement post in xmlDocument.Descendants("item"))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            string? name = ((string?) post.Element("title"))?.RemoveDiacritics();

            if (name.IsNullOrWhiteSpace())
            {
                continue;
            }

            string nodeName = name.Length > 250 ? name.Substring(0, 250) : name;
            string nodeCategory = (string?) post.Elements("category")
            .FirstOrDefault(c => c.Attribute("domain")?.Value == "post_tag") ?? "";

            try
            {
                if (string.Equals(nodeCategory, "Events"))
                {
                    PageEvent? existingPage = eventsOverview
                        .Children<PageEvent>()?
                        .FirstOrDefault(pageVacancy => pageVacancy.Name == nodeName);

                    IContent? page = existingPage is null
                        ? CreateIfNotExists(nodeName, eventsOverview.Id, PageEvent.ModelTypeAlias)
                        : _contentService.GetById(existingPage.Key);

                    if (page is null)
                    {
                        _logger.LogWarning("Could not import content {ID}", nodeName);
                        continue;
                    }

                    _eventsContentHelper.SetContent(page, post, cultures);

                    SaveOrPublish(page);
                }
                else if (string.Equals(nodeCategory, "Press Release"))
                {
                    PagePublication? existingPage = publicationsOverview
                        .Children<PagePublication>()?
                        .FirstOrDefault(pageVacancy => pageVacancy.Name == nodeName);

                    IContent? page = existingPage is null
                        ? CreateIfNotExists(nodeName, publicationsOverview.Id, PagePublication.ModelTypeAlias)
                        : _contentService.GetById(existingPage.Key);

                    if (page is null)
                    {
                        _logger.LogWarning("Could not import content {ID}", nodeName);
                        continue;
                    }

                    _publicationsContentHelper.SetContent(page, post, cultures);

                    SaveOrPublish(page);
                }
                else
                {
                    PageNews? existingPage = newsOverview
                        .Children<PageNews>()?
                        .FirstOrDefault(pageVacancy => pageVacancy.Name == nodeName);

                    IContent? page = existingPage is null
                        ? CreateIfNotExists(nodeName, newsOverview.Id, PageNews.ModelTypeAlias)
                        : _contentService.GetById(existingPage.Key);

                    if (page is null)
                    {
                        _logger.LogWarning("Could not import content {ID}", nodeName);
                        continue;
                    }

                    _newsContentHelper.SetContent(page, post, cultures);

                    SaveOrPublish(page);
                }
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
