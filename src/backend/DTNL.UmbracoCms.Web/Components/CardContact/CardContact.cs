using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class CardContact : ICard
{
    public required string FullName { get; set; }

    public string? Role { get; set; }

    public string? Location { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Text { get; set; }

    public Image? Image { get; set; }

    public string? CssClasses { get; set; }

    public static CardContact? Create(NestedBlockContactCard contactCard, string? cssClasses = null)
    {
        if (contactCard.FullName.IsNullOrWhiteSpace())
        {
            return null;
        }

        return new CardContact
        {
            FullName = contactCard.FullName,
            Role = contactCard.Role,
            Location = null, // TODO implement tags
            Email = contactCard.Email,
            PhoneNumber = contactCard.PhoneNumber,
            Text = contactCard.Description?.ToHtmlString(),
            Image = Image.Create(contactCard.Image, cssClasses: "card-contact__image"),
            CssClasses = cssClasses,
        };
    }
}
