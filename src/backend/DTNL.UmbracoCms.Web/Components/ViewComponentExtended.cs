using DTNL.UmbracoCms.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Dictionary;

namespace DTNL.UmbracoCms.Web.Components;

/// <summary>
///     Extends the <see cref="ViewComponent"/> class with properties that are commonly used when developing Umbraco components.
/// </summary>
public abstract class ViewComponentExtended : ViewComponent
{
    private NodeProvider? _nodeProvider;

    private ICultureDictionary? _cultureDictionary;

    public NodeProvider NodeProvider => _nodeProvider ??= HttpContext.RequestServices.GetRequiredService<NodeProvider>();

    public ICultureDictionary CultureDictionary => _cultureDictionary ??= HttpContext.RequestServices.GetRequiredService<ICultureDictionary>();
}
