using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class ButtonLink
{
    public Button? Button { get; set; }

    public string? Variant { get; set; }

    public static ButtonLink? Create(BlockListItem? buttonLink, string? cssClasses = null, string? svgIcon = null, string? jsHook = null)
    {
        if (buttonLink?.Content is not NestedBlockButtonLink button)
        {
            return null;
        }

        return new ButtonLink()
        {
            Button = Button.Create(button.Link)
                .With(b =>
                {
                    b.Class = cssClasses;
                    b.Icon = button.ButtonIcon?.LocalCrops.Src ?? svgIcon;
                    b.Variant = button.Variant;
                    b.Hook = jsHook;
                }),
        };
    }
}
