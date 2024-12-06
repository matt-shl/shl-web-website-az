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

    public string? Variant { get; set; }

    public bool Loop { get; set; } = true;

    private IEnumerable<VideoSizeSource>? Sources { get; init; }

    public string? SourcesJson => Sources?.Any() == true ? JsonSerializer.Serialize(Sources, JsonOptions) : null;

    public string? EmbedUrl { get; set; }

    public string? UploadDate { get; set; }

    public bool InView { get; set; } = true;

    public static Video? Create(
        NestedBlockVideo? block,
        string? css = null)
    {
        if (block?.Video?.FirstOrDefault()?.Content is not VideoMedia videoElement)
        {
            return null;
        }

        string? id = null;
        string platform = "native";
        IEnumerable<VideoSizeSource>? sources = GetSources(videoElement);

        return new Video
        {
            Id = id,
            InstanceId = $"{Random.Shared.Next()}",
            Platform = platform,
            Title = videoElement.Title,
            Description = videoElement.Description,
            Classes = css,
            Sources = sources,
            Thumbnail = Image.Create(block.Preview, width: 720, height: 400, cssClasses: "video__image"),
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
            Sources = GetSources(block)
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
