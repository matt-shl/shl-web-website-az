using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class Slogan
{
    public required string Text { get; set; }

    public bool AnimateOnScroll { get; set; }

    public string? CssClasses { get; set; }

    public static Slogan? Create(Umbraco.Cms.Web.Common.PublishedModels.Slogan? slogan, string? cssClasses = null)
    {
        if (slogan == null)
        {
            return null;
        }

        if (slogan.Text.IsNullOrWhiteSpace())
        {
            return null;
        }

        return new Slogan { Text = slogan.Text, AnimateOnScroll = slogan.AnimateOnScroll, CssClasses = cssClasses };
    }

    public static Slogan? Create(NestedBlockSlogan block, string? cssClasses = null)
    {
        Umbraco.Cms.Web.Common.PublishedModels.Slogan? slogan = block?.Slogan?.Select(block => block.Content).OfType<Umbraco.Cms.Web.Common.PublishedModels.Slogan>().FirstOrDefault();

        if (slogan is not { } scrollingText)
        {
            return null;
        }

        return new Slogan { Text = scrollingText?.Text ?? "", AnimateOnScroll = scrollingText!.AnimateOnScroll, CssClasses = cssClasses };
    }


}
