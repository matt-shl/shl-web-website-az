using DTNL.UmbracoCms.Web.Infrastructure.Filters;
using Microsoft.AspNetCore.Mvc;

namespace DTNL.UmbracoCms.Web.Api;

[ApiController]
[Route(ApiRoutePrefix + "/v{version:apiVersion}/[controller]")]
[ProblemDetailsExceptionFilter(Order = int.MinValue)]
public abstract class ApiControllerBase : ControllerBase
{
    public const string ApiRoutePrefix = "api";
}
