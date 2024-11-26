using DTNL.UmbracoCms.Web.Helpers.Aliases;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class FormOverlay
{
    public required Modal Modal { get; set; }

    public required PardotForm PardotForm { get; set; }

    public static FormOverlay? Create(NestedBlockEventDetails eventDetails, SiteSettings? settings)
    {
        if (PardotEventForm.Create(eventDetails) is not { } pardotEventForm)
        {
            return null;
        }

        Image? image = Image.Create(settings?.EventFormImage, style: "modal");

        return new FormOverlay
        {
            Modal = new Modal
            {
                Id = $"modal-form-{eventDetails.Key}",
                Size = image is null ? "aside" : "aside-with-image",
                Title = TranslationAliases.Forms.EventForm.Title,
                SubTitle = eventDetails.EventTitle,
                Image = image,
                KeepScrollPosition = true,
            },
            PardotForm = pardotEventForm,
        };
    }
}
