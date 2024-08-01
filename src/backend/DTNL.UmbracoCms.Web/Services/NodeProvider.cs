using DTNL.UmbracoCms.Web.Infrastructure.DependencyInjection;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Services;

[Scoped]
public class NodeProvider
{
    private readonly IUmbracoContextAccessor _umbracoContextAccessor;

    private PageHome? _homePage;
    private SiteSettings? _siteSettings;
    private IPublishedContent? _currentNode;

    public NodeProvider(IUmbracoContextAccessor umbracoContextAccessor)
    {
        _umbracoContextAccessor = umbracoContextAccessor;
    }

    /*
     * Because the HomePage and the SiteSettings of the page being rendered are used a lot,
     * we make them easier to use and also cache them for the duration of the request.
     */

    public PageHome? HomePage => _homePage ??= GetHomepage(GetCurrentNode());

    public SiteSettings? SiteSettings => _siteSettings ??= GetSiteSettings(HomePage);

    public IPublishedContent? CurrentNode => _currentNode ??= GetCurrentNode();

    internal void Reset()
    {
        _homePage = null;
        _siteSettings = null;
        _currentNode = null;
    }

    private static PageHome? GetHomepage(IPublishedContent? content)
    {
        return content?.AncestorOrSelf<PageHome>();
    }

    private static SiteSettings? GetSiteSettings(PageHome? homePage)
    {
        return homePage?.FirstChild<DataFolder>()?.FirstChild<SiteSettings>();
    }

    public IPublishedContent? GetCurrentNode()
    {
        return _umbracoContextAccessor.GetRequiredUmbracoContext().PublishedRequest?.PublishedContent;
    }
}
