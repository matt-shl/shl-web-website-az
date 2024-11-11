using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Helpers;

/// <summary>
/// Helper class for Error Pages related functionality.
/// </summary>
public static class ErrorPagesHelper
{
    /// <summary>
    /// Error routes prefix.
    /// </summary>
    public const string ErrorPathPrefix = "/#error";

    /// <summary>
    /// Determines if the provided path is an error path that matches the <see cref="ErrorPathPrefix"/>.
    /// </summary>
    public static bool IsErrorPath(string path, out int errorCode)
    {
        errorCode = default;
        string errorPathStart = $"{ErrorPathPrefix}/";
        if (!path.StartsWith(errorPathStart, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        string errorCodeStr = path[errorPathStart.Length..].TrimEnd('/');

        return int.TryParse(errorCodeStr, out errorCode);
    }

    /// <summary>
    /// Tries to find the error page node based on the request domain and error code.
    /// </summary>
    /// <returns>The corresponding error page or null if not found.</returns>
    /// <remarks>If the request domain is not set, it will search in the first root node.</remarks>
    public static IPublishedContent? FindErrorPage(this IUmbracoContext umbracoContext, DomainAndUri? domain, int errorCode)
    {
        int? siteId = domain?.ContentId;

        IPublishedContent? siteRoot = umbracoContext.Content?.GetById(siteId ?? -1) ?? umbracoContext.Content?.GetAtRoot().FirstOrDefault();

        SiteSettings? siteSettings = siteRoot?.Descendant<SiteSettings>();

        if (siteSettings == null)
        {
            return null;
        }

        return errorCode switch
        {
            StatusCodes.Status404NotFound => siteSettings.UmbracoError404,
            _ => siteSettings.UmbracoError500,
        };
    }
}
