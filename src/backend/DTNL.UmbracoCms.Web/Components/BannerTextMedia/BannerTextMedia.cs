using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class BannerTextMedia
{
    public enum Position
    {
        Start,
    }

    public required string Title { get; set; }

    public required string Description { get; set; }

    public string? MediaPosition { get; set; }

    public Button? PrimaryButton { get; set; }

    public Button? SecondaryButton { get; set; }

    public Video? Video { get; set; }

    public Image? ImageData { get; set; }

    public string? Theme { get; set; }

    public static BannerTextMedia? Create(NestedBlockTextMediaBanner textMediaBanner)
    {
        if (textMediaBanner.Title.IsNullOrWhiteSpace() || textMediaBanner.Description.IsNullOrWhiteSpace())
        {
            return null;
        }

        NestedBlockImage? imageContent = (NestedBlockImage?) textMediaBanner.Image?.FirstOrDefault()?.Content;
        VideoMedia? videoContent = (VideoMedia?) textMediaBanner.Video?.FirstOrDefault()?.Content;

        return new BannerTextMedia
        {
            Title = textMediaBanner.Title,
            Description = textMediaBanner.Description,
            MediaPosition = string.Equals(textMediaBanner.MediaPosition, "right", StringComparison.OrdinalIgnoreCase) ? "end" : "start",
            PrimaryButton = Button
                .Create(textMediaBanner.PrimaryButton).With(b =>
                {
                    b.Icon = Helpers.Aliases.SvgAliases.Icons.ArrowTopRight;
                    b.Variant = "primary";
                }),
            SecondaryButton = Button
                .Create(textMediaBanner.SecondaryButton)
                .With(b =>
                {
                    b.Icon = Helpers.Aliases.SvgAliases.Icons.ArrowTopRight;
                    b.Variant = "secondary";
                }),
            Video = Video.Create(videoContent)
            .With(v =>
            {
                v.Id = videoContent?.Title?.Trim().ToLowerInvariant().Replace(" ", "-");
                v.Description = videoContent?.Description;
                v.TotalTime = videoContent?.TotalTime;
                v.Variant = "modal";
            }),
            ImageData = Image
                .Create(imageContent?.Image, style: "in-grid-banner-image")
                .With(i =>
                {
                    i.CardOverlay = videoContent is null
                        ? null
                        : new CardOverlay
                        {
                            Video = new Video
                            {
                                Id = videoContent.Title?.Trim().ToLowerInvariant().Replace(" ", "-"),
                                InstanceId = videoContent.Title?.Trim().ToLowerInvariant().Replace(" ", "-") ?? "",
                                Platform = "native",
                                TotalTime = videoContent.TotalTime ?? "",
                                Variant = "modal",
                            },
                            Position = "start",
                            Visible = true,
                        };
                    i.ImageHolderButton = videoContent != null;
                    i.ImageHolderAttributes = new Dictionary<string, string?>
                    {
                        ["aria-label"] = "Open video Modal",
                        ["aria-controls"] = videoContent != null ? $"modal-video-{videoContent.Title?.Trim().ToLowerInvariant().Replace(" ", "-")}" : null,
                    };
                    i.ObjectFit = true;
                    i.Caption = videoContent is not null ? videoContent.Description : imageContent?.Caption;
                }),
        };
    }
}
