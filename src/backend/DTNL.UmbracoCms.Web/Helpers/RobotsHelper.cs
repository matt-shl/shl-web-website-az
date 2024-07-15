using System.Net.Mime;
using DTNL.UmbracoCms.Web.Infrastructure.Configuration.Options;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DTNL.UmbracoCms.Web.Helpers;

/// <summary>
/// Helper class for Robots related functionality.
/// </summary>
public static class RobotsHelper
{
    /// <summary>
    /// Registers the Robots file handler.
    /// </summary>
    public static IEndpointRouteBuilder UseRobots(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet("/robots.txt", GenerateRobots);
        return endpointRouteBuilder;
    }

    /// <summary>
    /// Writes the robots content to the response body.
    /// </summary>
    /// <remarks>You can configure the crawlable domains using the Application options.</remarks>
    public static Task GenerateRobots(HttpContext context, [FromServices] IOptionsMonitor<ApplicationOptions> applicationOptions)
    {
        Uri currentUri = new(context.Request.GetDisplayUrl());
        string output = applicationOptions.CurrentValue.IsCrawlableUrl(currentUri) switch
        {
            true => "User-agent: *\n" +
                    "Allow: /\n" +
                    $"Sitemap: {context.Request.Scheme}://{context.Request.Host}/sitemap.xml",

            _ => "User-agent: *\n" +
                 "Disallow: /",
        };

        context.Response.ContentType = MediaTypeNames.Text.Plain;
        return context.Response.WriteAsync(output);
    }
}
