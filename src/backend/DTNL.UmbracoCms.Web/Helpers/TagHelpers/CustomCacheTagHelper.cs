using System.Text.Encodings.Web;
using DTNL.UmbracoCms.Web.Infrastructure;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DTNL.UmbracoCms.Web.Helpers.TagHelpers;

/// <summary>
/// Custom <see cref="CacheTagHelper"/> to automatically disables the helper based on <see cref="CacheManager"/>'s logic.
/// </summary>
[HtmlTargetElement("cache")]
public class CustomCacheTagHelper : CacheTagHelper
{
    private readonly ICacheManager _cacheManager;

    public CustomCacheTagHelper(ICacheManager cacheManager, CacheTagHelperMemoryCacheFactory factory, HtmlEncoder htmlEncoder)
        : base(factory, htmlEncoder)
    {
        _cacheManager = cacheManager;
    }

    public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        Enabled = Enabled && _cacheManager.ShouldRequestBeCached(ViewContext.HttpContext);

        return base.ProcessAsync(context, output);
    }
}
