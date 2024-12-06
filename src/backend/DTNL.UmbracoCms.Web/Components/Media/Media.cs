using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Web.Common.PublishedModels;
using NestedBlockVideo = Umbraco.Cms.Web.Common.PublishedModels.NestedBlockVideo;

namespace DTNL.UmbracoCms.Web.Components;

public class Media
{
    public Image? Image { get; set; }

    public Video? Video { get; set; }

    public static Media? Create(string? media)
    {
        if (Image.Create(media) is not { } image)
        {
            return null;
        }

        return new Media
        {
            Image = Image.Create(media).With(i =>
            {
                i.Classes = "media-section__image";
                i.Caption = image.Caption;
            }),
        };
    }

    public static Media? Create(NestedBlockVideo? videoBlock)
    {
        if (videoBlock is null)
        {
            return null;
        }

        VideoMedia? videoContent = (VideoMedia?) videoBlock.Video?.FirstOrDefault()?.Content;
        Video? video = Video.Create(videoContent).With(v =>
        {
            v.Id = videoContent?.Title?.Trim().ToLowerInvariant().Replace(" ", "-");
            v.Description = videoContent?.Description;
            v.TotalTime = videoContent?.TotalTime;
            v.Variant = "modal";
        });

        return new Media
        {
            Image = Image
                .Create(videoBlock.Preview)
                .With(i =>
                {
                    i.Classes = "media-section__image";
                    i.CardOverlay = videoContent is null
                        ? null
                        : new CardOverlay
                        {
                            Video = video,
                            Position = "start",
                            Visible = true,
                        };
                    i.ImageHolderButton = videoContent != null;
                    i.ImageHolderAttributes = new Dictionary<string, string?>
                    {
                        ["aria-label"] = "Open video Modal",
                        ["aria-controls"] = video != null ? $"{video.InstanceId}" : null,
                    };
                    i.ObjectFit = true;
                    i.Caption = videoBlock.Caption;
                }),
            Video = video,
        };
    }
}
