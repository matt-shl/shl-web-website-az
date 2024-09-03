using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public partial class HistoryTimeline
{
    public string? Title { get; set; }

    public required List<HistoryTimelineItem> Items { get; set; }

    public required Odometer Odometer { get; set; }

    public static HistoryTimeline? Create(NestedBlockHistoryTimeline historyTimeline)
    {
        List<HistoryTimelineItem> historyTimelineItems = historyTimeline.Items
            .Using(i => i.Content as NestedBlockHistoryTimelineItem)
            .Using(HistoryTimelineItem.Create)
            .ToList();

        if (historyTimelineItems.Count == 0)
        {
            return null;
        }

        return new HistoryTimeline
        {
            Title = historyTimeline.Title,
            Items = historyTimelineItems,
            Odometer = Odometer
                .Create(historyTimelineItems.First().Year)
                .With(o => o.Id = "history-timeline"),
        };
    }
}
