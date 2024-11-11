using Umbraco.Cms.Core.Models.PublishedContent;

namespace DTNL.UmbracoCms.Web.Helpers.Extensions;

public static class VariationContextAccessorExtensions
{
    public static bool SetVariationContext(this IVariationContextAccessor variationContextAccessor, string? culture, string? segment = null)
    {
        VariationContext? variationContext = variationContextAccessor.VariationContext;
        if (variationContext?.Culture == (culture ?? "") && variationContext?.Segment == (segment ?? ""))
        {
            // We already have the correct variation context, nothing to do here
            return false;
        }

        variationContextAccessor.VariationContext = new VariationContext(culture, segment);
        return true;
    }
}
