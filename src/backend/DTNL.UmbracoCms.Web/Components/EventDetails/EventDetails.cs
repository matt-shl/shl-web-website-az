using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class EventDetails
{
    public string? Title { get; set; }

    public string? Location { get; set; }

    public string? Time { get; set; }

    public Button? LinkButton { get; set; }

    public Media? Media { get; set; }

    public FormOverlay? FormOverlay { get; set; }

    public static EventDetails? Create(NestedBlockEventDetails? eventDetails, SiteSettings? settings)
    {
        if (eventDetails is null)
        {
            return null;
        }

        Button? linkButton = Button.Create(eventDetails.EventLink);
        FormOverlay? formOverlay = FormOverlay.Create(eventDetails, settings);

        if (linkButton is not null)
        {
            linkButton.Controls = formOverlay?.Modal.Id;
        }

        return new EventDetails
        {
            Title = eventDetails.EventTitle,
            Location = eventDetails.EventLocationInfo?.ToHtmlString(),
            Time = eventDetails.EventTime?.ToHtmlString(),
            LinkButton = linkButton,
            Media = Media.Create(eventDetails.EventImage),
            FormOverlay = formOverlay,
        };
    }
}
