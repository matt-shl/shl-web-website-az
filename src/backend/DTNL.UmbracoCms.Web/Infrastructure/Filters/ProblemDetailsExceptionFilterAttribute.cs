using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace DTNL.UmbracoCms.Web.Infrastructure.Filters;

/// <summary>
/// Handles action exceptions responding with an <see cref="ProblemDetails"/> response with the appropriate Content-Type.
/// </summary>
/// <remarks>Logging the exception isn't handled here, as the <see cref="ExceptionHandlerMiddleware"/> does it already.</remarks>
public partial class ProblemDetailsExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        ILogger logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<ProblemDetailsExceptionFilterAttribute>>();
        UnhandledException(logger, context.Exception);

        ProblemDetailsFactory problemDetailsFactory = context.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
        ProblemDetails problemDetails = problemDetailsFactory.CreateProblemDetails(context.HttpContext, title: "Unknown Error");

        // Use an object result so the correct formatter is used (e.g. Json, Xml...)
        context.Result = new ObjectResult(problemDetails) { StatusCode = StatusCodes.Status500InternalServerError };
        context.ExceptionHandled = true;
    }

    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Error,
        Message = "An unhandled exception has occurred while executing the API request.")]
    protected static partial void UnhandledException(ILogger logger, Exception ex);
}
