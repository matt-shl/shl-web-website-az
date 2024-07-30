using DTNL.UmbracoCms.Web.Helpers;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Web;

namespace DTNL.UmbracoCms.Web.Infrastructure.ContentFinders;

/// <summary>
/// Provides an implementation of <see cref="IContentLastChanceFinder"/> that sets the 404 or error page.
/// </summary>
public class LastChanceContentFinder : IContentLastChanceFinder
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUmbracoContextAccessor _umbracoContextAccessor;
    private readonly IVariationContextAccessor _variationContextAccessor;
    private readonly IDefaultCultureAccessor _defaultCultureAccessor;

    public LastChanceContentFinder(
        IHttpContextAccessor httpContextAccessor,
        IUmbracoContextAccessor umbracoContextAccessor,
        IVariationContextAccessor variationContextAccessor,
        IDefaultCultureAccessor defaultCultureAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _umbracoContextAccessor = umbracoContextAccessor;
        _variationContextAccessor = variationContextAccessor;
        _defaultCultureAccessor = defaultCultureAccessor;
    }

    /// <summary>
    /// Tries to assign the correct error page and status code based on the <c>PublishedRequest</c> and original <c>HttpRequest</c>.
    /// </summary>
    /// <param name="request">The <c>PublishedRequest</c>.</param>
    /// <returns>A value indicating whether we were able to set an error page/status code.</returns>
    public Task<bool> TryFindContent(IPublishedRequestBuilder request)
    {
        if (!ErrorPagesHelper.IsErrorPath(request.Uri.LocalPath, out int statusCode))
        {
            statusCode = StatusCodes.Status404NotFound;
        }

        if (_httpContextAccessor.HttpContext?.Features.Get<IStatusCodeReExecuteFeature>() is { } statusCodeReExecuteFeature)
        {
            // Re-execution - Restore the original request path
            // While not strictly necessary, this ensure that any code that references the request URL
            // transparently gets the original values.
            _httpContextAccessor.HttpContext.Request.Path = statusCodeReExecuteFeature.OriginalPath;
            _httpContextAccessor.HttpContext.Request.PathBase = statusCodeReExecuteFeature.OriginalPathBase;
            _httpContextAccessor.HttpContext.Request.QueryString = QueryString.FromUriComponent(statusCodeReExecuteFeature.OriginalQueryString ?? "");
        }

        SetDomainIfNull(request);
        IPublishedContent? errorPage = _umbracoContextAccessor.GetRequiredUmbracoContext().FindErrorPage(request.Domain, statusCode);

        request
            .SetPublishedContent(errorPage)
            .SetResponseStatus(statusCode)
            .SetNoCacheHeader(true);

        return Task.FromResult(errorPage != null);
    }

    /// <summary>
    /// Sets the request builder domain, if not already set, to the original's request domain or the closest to it (based on culture and host).
    /// </summary>
    private void SetDomainIfNull(IPublishedRequestBuilder request)
    {
        if (request.Domain != null)
        {
            return;
        }

        HttpContext httpContext = _httpContextAccessor.HttpContext!;
        IUmbracoContext umbCtx = _umbracoContextAccessor.GetRequiredUmbracoContext();

        // Retrieve original uri
        Uri currentUri = new(httpContext.Request.GetDisplayUrl());
        string originalPath = httpContext.Features.Get<IHttpRequestFeature>()?.RawTarget ?? currentUri.PathAndQuery;
        Uri originalUri = new(currentUri, originalPath);

        List<Domain> allDomains = umbCtx.Domains?.GetAll(true).ToList() ?? [];

        // Get the domain corresponding to the original request or fallback to the most similar.
        DomainAndUri? domain = DomainUtilities.SelectDomain(allDomains, originalUri, filter: (domains, _, _, _) =>
            domains
                .OrderByDescending(d => d.Culture == _defaultCultureAccessor.DefaultCulture)
                .ThenByDescending(d => d.Uri.Authority == originalUri.Authority)
                .First());

        if (domain != null)
        {
            // Update domain and culture
            request.SetDomain(domain);
            _ = _variationContextAccessor.SetVariationContext(domain.Culture);
        }
    }
}
