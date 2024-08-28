using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class CardKnowledge : ICard
{
    public required string Title { get; set; }

    public string? Tag { get; set; }

    public string? Text { get; set; }

    public Image? Image { get; set; }

    public string? Url { get; set; }

    public string? CssClasses { get; set; }

    public string Element => !Url.IsNullOrEmpty() ? "a" : "article";

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

    public static CardKnowledge? CreateOverview(ICompositionKnowledgePage overviewPage, string? cssClasses = null)
    {
        if (overviewPage is not ICompositionBasePage page)
        {
            return null;
        }

        return new CardKnowledge
        {
            Title = page.GetTitle(),
            Tag = overviewPage?.PageType?.FirstOrDefault(),
            Text = page.GetCardDescription(),
            Image = Image.Create(page.GetCardImage(), cssClasses: "card-knowledge__image", style: "card-knowledge"),
            Url = page.Url(),
            CssClasses = cssClasses,
        };
    }
}
