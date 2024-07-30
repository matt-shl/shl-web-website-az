using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class Card
{
    public string? Title { get; set; }

    public static Card Create(ICompositionBasePage basePage)
    {
        return new Card
        {
            Title = null
        };
    }
}
