using DTNL.UmbracoCms.Web.Components.Hero;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class Slogan : IHero
{
    public required string Text { get; set; }

    public string? Variant { get; set; }

    public string? CssClasses { get; set; }

    public static Slogan? Create(string? slogan, string? cssClasses = null)
    {
        if (slogan.IsNullOrWhiteSpace())
        {
            return null;
        }

        return new Slogan { Text = slogan, CssClasses = cssClasses };
    }

    public static Slogan? Create(NestedBlockSlogan? slogan, string? cssClasses = null)
    {
        return Create(slogan?.Text, cssClasses);
    }

    public static Slogan? Create(NestedBlockSloganHero? slogan, string? cssClasses = null)
    {
        return Create(slogan?.Title, cssClasses).With(s => s.Variant = "in-hero");
    }
}
