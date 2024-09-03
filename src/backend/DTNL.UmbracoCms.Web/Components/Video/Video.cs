using System.Text.Json;
using System.Text.Json.Serialization;
using Umbraco.Cms.Web.Common.PublishedModels;
using NestedBlockVideo = Umbraco.Cms.Web.Common.PublishedModels.NestedBlockVideo;

namespace DTNL.UmbracoCms.Web.Components;

public class Video
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerOptions.Default) { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

    public required string InstanceId { get; set; }

    public required string Platform { get; set; }

    public string? Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public Image? Thumbnail { get; set; }

    public string? TotalTime { get; set; }

    public string? StartTime { get; set; }

    public string? Classes { get; set; }

    public bool Muted { get; set; }

    public bool Autoplay { get; set; }

    public bool AutoPause { get; set; } = true;

    public bool PlaysInLine { get; set; } = true;

    public bool Controls { get; set; } = true;

    public bool CustomControls { get; set; } = true;


    public bool Loop { get; set; } = true;

    private IEnumerable<VideoSizeSource>? Sources { get; init; }

    public string? SourcesJson => Sources?.Any() == true ? JsonSerializer.Serialize(Sources, JsonOptions) : null;

    public IEnumerable<VideoClosedCaptions>? ClosedCaptions { get; set; }

    public string? CaptionsJson => ClosedCaptions?.Any() == true ? JsonSerializer.Serialize(ClosedCaptions, JsonOptions) : null;

    public string? EmbedUrl { get; set; }

    public string? UploadDate { get; set; }

    public bool InView { get; set; } = true;

    public static Video? Create(
        NestedBlockVideo? block,
        string? css = null)
    {
        if (block?.Video?.FirstOrDefault()?.Content is not { } videoElement)
        {
            return null;
        }

        string? id = null;
        string platform;
        IEnumerable<VideoSizeSource>? sources = null;

        switch (videoElement)
        {
            case NestedBlockVideoVimeo vimeoVideo:
                id = vimeoVideo.VideoId;
                platform = "vimeo";
                break;
            case NestedBlockVideoYoutube youtubeVideo:
                id = youtubeVideo.VideoId;
                platform = "youtube";
                break;
            case NestedBlockVideoNativeUrl nativeUrlVideo:
                sources = GetSources(nativeUrlVideo);
                platform = "native";
                break;
            case NestedBlockVideoNativeCms nativeCmsVideo:
                sources = GetSources(nativeCmsVideo);
                platform = "native";
                break;

            case VideoMedia videoMedia:
                sources = GetSources(videoMedia);
                platform = "native";
                break;
            default:
                throw new NotImplementedException($"Video type {videoElement.GetType().Name} not implemented");
        }

        return new Video
        {
            Id = id,
            InstanceId = $"{Random.Shared.Next()}",
            Platform = platform,
            Title = block.Title,
            Description = block.Description,
            Classes = css,
            Sources = sources,
            Thumbnail = Image.Create(block.Preview, width: 720, height: 400, cssClasses: "video__image"),
        };
    }

    public static Video? Create(
        NestedBlockVideoNativeUrl? block,
        string? css = null)
    {
        if (block is null)
        {
            return null;
        }

        string? id = null;
        string platform;

        platform = "native";

        return new Video
        {
            Id = id,
            InstanceId = $"{Random.Shared.Next()}",
            Platform = platform,
            Classes = css,
            Sources = GetSources(block),
        };
    }

    public static Video? Create(
       VideoMedia? block,
       string? css = null)
    {
        if (block is null)
        {
            return null;
        }

        string? id = null;
        string platform;

        platform = "native";

        return new Video
        {
            Id = id,
            InstanceId = $"{Random.Shared.Next()}",
            Platform = platform,
            Classes = css,
            Sources = GetSources(block),
            ClosedCaptions = block?.ClosedCaptions?.Select(c => new VideoClosedCaptions
            {
                Url = (block.ClosedCaptions?.FirstOrDefault()?.Content as ClosedCaptions)?.Url ?? "",
                Kind = (block.ClosedCaptions?.FirstOrDefault()?.Content as ClosedCaptions)?.Kind,
                Label = (block.ClosedCaptions?.FirstOrDefault()?.Content as ClosedCaptions)?.Label,
                Lang = (block.ClosedCaptions?.FirstOrDefault()?.Content as ClosedCaptions)?.Lang,

            }
            ),
        };
    }

    private static IEnumerable<VideoSizeSource> GetSources(VideoMedia nativeUrlVideo)
    {
        return nativeUrlVideo.Sources?
                   .Select(s => s.Content)
                   .Cast<VideoSourceUrl>()
                   .Select(s => GetSource(s.VideoLink, s.SourceSize))
                   .WhereNotNull()
               ?? [];
    }

    private static IEnumerable<VideoSizeSource> GetSources(NestedBlockVideoNativeUrl nativeUrlVideo)
    {
        return nativeUrlVideo.Sources?
                   .Select(s => s.Content)
                   .Cast<VideoSourceUrl>()
                   .Select(s => GetSource(s.VideoLink, s.SourceSize))
                   .WhereNotNull()
               ?? [];
    }

    private static IEnumerable<VideoSizeSource> GetSources(NestedBlockVideoNativeCms nativeCmsVideo)
    {
        return nativeCmsVideo.Sources?
            .Select(s => s.Content)
            .Cast<VideoSourceCms>()
            .Select(s => GetSource(s.Video?.Url(), s.SourceSize))
            .WhereNotNull()
               ?? [];
    }

    private static VideoSizeSource? GetSource(string? videoUrl, string? sourceSize)
    {
        if (videoUrl is null || GetSourceSize(sourceSize) is not { } size)
        {
            return null;
        }

        return new VideoSizeSource(size, videoUrl);
    }

    private static int? GetSourceSize(string? sourceSize)
    {
        sourceSize = new string(sourceSize?.TakeWhile(char.IsNumber).ToArray());

        return int.TryParse(sourceSize, out int size) ? size * 16 / 9 : null;
    }
}
