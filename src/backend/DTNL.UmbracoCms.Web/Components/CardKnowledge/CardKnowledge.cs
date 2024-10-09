using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class CardKnowledge : ICard, IOverviewItem
{
    public required string Title { get; set; }

    public string? Tag { get; set; }

    public string? Text { get; set; }

    public Image? Image { get; set; }

    public string? Url { get; set; }

    public string? CssClasses { get; set; }

    public bool HasUrl => !Url.IsNullOrWhiteSpace();

    public string Element => !Url.IsNullOrWhiteSpace() ? "a" : "article";

    public static CardKnowledge? Create(NestedBlockPageCard pageCard, string? cssClasses = null)
    {
        if (pageCard.Page is not ICompositionBasePage page)
        {
            return null;
        }

        return new CardKnowledge
        {
            Title = page.GetTitle(),
            Tag = page.GetCategory(),
            Text = page.GetCardDescription(),
            Image = Image.Create(page.GetCardImage(), cssClasses: "card-knowledge__image", style: "card-knowledge"),
            Url = page.Url(),
            CssClasses = cssClasses,
        };
    }

    public static CardKnowledge Create(ICompositionBasePage page)
    {
        return new CardKnowledge
        {
            Title = page.GetTitle(),
            Tag = (page as ICompositionCardDetails)?.CardCategory?.FirstOrDefault(),
            Text = page.GetCardDescription(),
            Image = Image.Create(page.GetCardImage(), cssClasses: "card-knowledge__image", style: "card-knowledge"),
            Url = page.Url(),
        };
    }
}
