using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Core.Models;

namespace DTNL.UmbracoCms.Web.Components;

public class Media
{
    public Image? Image { get; set; }

    public Video? Video { get; set; }

    public static Media? Create(MediaWithCrops? mediaWithCrops)
    {
        if (Image.Create(mediaWithCrops) is not { } image)
        {
            return null;
        }

        return new Media
        {
            Image = image.With(i =>
            {
                i.Classes = "media-section__image";
                i.Caption = image.Caption;
            }),
        };
    }
}
