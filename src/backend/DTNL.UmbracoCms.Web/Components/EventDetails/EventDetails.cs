using DTNL.UmbracoCms.Web.Helpers.Aliases;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class EventDetails
{
    public string? Title { get; set; }

    public string? Location { get; set; }

    public string? Time { get; set; }

    public Button? RegisterButton { get; set; }

    public Media? Media { get; set; }

    public FormOverlay? FormOverlay { get; set; }

    public static EventDetails? Create(NestedBlockEventDetails? eventDetails, SiteSettings? settings)
    {
        if (eventDetails is null)
        {
            return null;
        }

        Button? registerButton = new Button
        {
            Element = "button",
            Class = "event-detail__col event-detail__col--cta",
            Variant = "primary",
            Label = TranslationAliases.Forms.EventForm.Title,
            Icon = SvgAliases.Icons.ArrowTopRight,
        };
        FormOverlay? formOverlay = FormOverlay.Create(eventDetails, settings);

        if (registerButton is not null)
        {
            registerButton.Controls = formOverlay?.Modal.Id;
        }

        return new EventDetails
        {
            Title = eventDetails.EventTitle,
            Location = eventDetails.EventLocationInfo?.ToHtmlString(),
            Time = eventDetails.EventTime?.ToHtmlString(),
            RegisterButton = registerButton,
            Media = Media.Create(eventDetails.EventImage),
            FormOverlay = formOverlay,
        };
    }
}
