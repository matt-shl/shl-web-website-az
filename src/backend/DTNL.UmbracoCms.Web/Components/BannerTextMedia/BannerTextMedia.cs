using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class BannerTextMedia
{
    public enum Position
    {
        Start,
    }

    public string? AnchorId { get; set; }

    public string? AnchorTitle { get; set; }

    public required string Title { get; set; }

    public required string Description { get; set; }

    public string? MediaPosition { get; set; }

    public Button? PrimaryButton { get; set; }

    public Button? SecondaryButton { get; set; }

    public Video? Video { get; set; }

    public NestedBlockImage? ImageContent { get; set; }

    public string? Theme { get; set; }

    public static BannerTextMedia? Create(NestedBlockTextMediaBanner textMediaBanner)
    {
        if (textMediaBanner.Title.IsNullOrWhiteSpace() || textMediaBanner.Description.IsNullOrWhiteSpace())
        {
            return null;
        }

        return new BannerTextMedia
        {
            AnchorId = textMediaBanner.AnchorId,
            AnchorTitle = textMediaBanner.AnchorTitle,
            Title = textMediaBanner.Title,
            Description = textMediaBanner.Description,
            MediaPosition = textMediaBanner.MediaPosition,
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
            Video = Video.Create((VideoMedia?) textMediaBanner.Video?.FirstOrDefault()?.Content)
            .With(v =>
            {
                v.Id = (textMediaBanner?.Video?.FirstOrDefault()?.Content as VideoMedia)?.Title?.Trim().ToLower().Replace(" ", "-");
                v.Description = (textMediaBanner?.Video?.FirstOrDefault()?.Content as VideoMedia)?.Description;
                v.Autoplay = false;
                v.Muted = true;
                v.TotalTime = (textMediaBanner?.Video?.FirstOrDefault()?.Content as VideoMedia)?.TotalTime;
            }),
            ImageContent = (NestedBlockImage?) textMediaBanner.Image?.FirstOrDefault()?.Content,
        };
    }
}
