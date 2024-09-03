using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public partial class HistoryTimeline
{
    public class HistoryTimelineItem
    {
        public int Year { get; set; }

        public string? ShortTitle { get; set; }

        public string? Title { get; set; }

        public string? Text { get; set; }

        public required Image Image { get; set; }

        public required Image ThumbnailImage { get; set; }

        public static HistoryTimelineItem Create(NestedBlockHistoryTimelineItem historyTimelineItem)
        {
            return new()
            {
                Year = historyTimelineItem.Year,
                Title = historyTimelineItem.Title,
                ShortTitle = historyTimelineItem.ShortTitle,
                Text = historyTimelineItem.Text?.ToHtmlString(),
                Image = Image.Create(historyTimelineItem.Image, style: "history-timeline")!,
                ThumbnailImage = Image.Create(historyTimelineItem.Image, style: "history-timeline-mini")!,
            };
        }
    }
}
