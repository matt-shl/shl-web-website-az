using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class VideoModal
{
    public required Video Video { get; set; }

    public static VideoModal? Create(NestedBlockVideo? block)
    {
        if (Video.Create(block) is not { } video)
        {
            return null;
        }

        video.Controls = true;

        return new VideoModal
        {
            Video = video,
        };
    }
}
