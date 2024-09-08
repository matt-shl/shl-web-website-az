using Umbraco.Cms.Core.Models.PublishedContent;

namespace DTNL.UmbracoCms.Web.Components.NestedBlock;

public class NestedBlockHistoryTimeline : NestedBlockWithInner
{
    protected override HistoryTimeline? GetInnerComponent(IPublishedElement block)
    {
        if (block is not Umbraco.Cms.Web.Common.PublishedModels.NestedBlockHistoryTimeline historyTimelineBlock)
        {
            return null;
        }

        if (HistoryTimeline.Create(historyTimelineBlock) is not { } historyTimeline)
        {
            return null;
        }

        LayoutSection.CssClasses = "c-history-timeline";
        LayoutSection.JsHook = "history-timeline";

        return historyTimeline;
    }
}
