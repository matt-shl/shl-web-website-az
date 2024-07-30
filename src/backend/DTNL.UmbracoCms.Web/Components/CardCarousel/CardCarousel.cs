using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class CardCarousel
{
    public string? Title { get; set; }

    public required List<Card> Cards { get; set; }

    public CardCarousel Create(ICompositionBasePage basePage)
    {
        return new CardCarousel
        {
            Title = null,
            Cards = [],
        };
    }
}
