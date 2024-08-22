using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class EventDetails
{
    public string? ThemeCssClasses { get; set; }

    public string? Title { get; set; }

    public string? Location { get; set; }

    public string? Time { get; set; }

    public Button? Link { get; set; }

    public static EventDetails? Create(NestedBlockEventDetails? eventDetails)
    {
        if (eventDetails is null)
        {
            return null;
        }

        return new EventDetails
        {
            Title = eventDetails.EventTitle,
            Location = eventDetails.EventLocationInfo?.ToHtmlString(),
            Time = eventDetails.EventTime?.ToHtmlString(),
            Link = Button.Create(eventDetails.EventUrl),
        };
    }
}
