using DTNL.UmbracoCms.Web.Components.Hero;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class Slogan : IHero
{
    public required string Title { get; set; }

    public string? Variant { get; set; }

    public string? CssClasses { get; set; }

    public static Slogan? Create(string? slogan, string? cssClasses = null)
    {
        if (slogan.IsNullOrWhiteSpace())
        {
            return null;
        }

        return new Slogan { Title = slogan, CssClasses = cssClasses };
    }

    public static Slogan? Create(NestedBlockSlogan? slogan, string? cssClasses = null)
    {
        return Create(slogan?.Text, cssClasses);
    }
}
