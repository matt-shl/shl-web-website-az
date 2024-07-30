using DTNL.UmbracoCms.Web.Infrastructure.Filters;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;

namespace DTNL.UmbracoCms.Web.Controllers;

/// <summary>
/// Represents the default front-end rendering controller.
/// </summary>
/// <remarks>Can be overriden using controller hijacking if required.</remarks>
[CustomResponseCache(Duration = 5 * 60, ServerDuration = 10 * 60)]
public sealed class DefaultRenderController : RenderController
{
    public DefaultRenderController(
        ILogger<RenderController> logger,
        ICompositeViewEngine compositeViewEngine,
        IUmbracoContextAccessor umbracoContextAccessor
    )
        : base(logger, compositeViewEngine, umbracoContextAccessor)
    {
    }

    /// <inheritdoc />
    /// <remarks>
    /// Sets the appropriate RequestTelemetry.
    /// </remarks>
    public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        IPublishedRequest publishedRequest = UmbracoRouteValues.PublishedRequest;

        switch (publishedRequest.GetRouteResult())
        {
            case UmbracoRouteResult.Redirect:
                SetRequestTelemetryName("DefaultRender/Redirect");
                break;
            case UmbracoRouteResult.Success or UmbracoRouteResult.NotFound when publishedRequest.ResponseStatusCode is StatusCodes.Status404NotFound:
                // Whether we found a template or not, if the status is 404 we set the name to the request path
                string path = HttpContext.Features.Get<IStatusCodeReExecuteFeature?>()?.OriginalPath ?? HttpContext.Request.Path.Value ?? "NotFound";
                SetRequestTelemetryName(path);
                break;
            case UmbracoRouteResult.Success when (publishedRequest.ResponseStatusCode ?? 200) is >= StatusCodes.Status200OK and < StatusCodes.Status300MultipleChoices:
                SetRequestTelemetryName($"DefaultRender/{publishedRequest.PublishedContent?.ContentType?.Alias ?? "Unknown"}/{publishedRequest.GetTemplateAlias() ?? "Unknown"}");
                break;
            default:
                break;
        }

        return base.OnActionExecutionAsync(context, next);
    }

    // Sets the Application Insights request name, so it's easy to differentiate requests
    private void SetRequestTelemetryName(string action)
    {
        RequestTelemetry? requestTelemetry = HttpContext.Features.Get<RequestTelemetry?>();
        if (requestTelemetry == null)
        {
            return;
        }

        requestTelemetry.Name = $"{HttpContext.Request.Method} {action}";
    }
}
