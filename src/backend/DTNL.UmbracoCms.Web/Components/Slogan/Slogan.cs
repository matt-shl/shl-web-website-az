using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class Slogan
{
    public required string Text { get; set; }

    public string? CssClasses { get; set; }

    public static Slogan? Create(NestedBlockSlogan? slogan, string? cssClasses = null)
    {
        if (slogan == null)
        {
            return null;
        }

        if (slogan.Text.IsNullOrWhiteSpace())
        {
            return null;
        }

        return new Slogan { Text = slogan.Text, CssClasses = cssClasses };
    }
}
