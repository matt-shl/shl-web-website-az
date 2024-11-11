using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class CardNumber : ICard
{
    public required string Title { get; set; }

    public string? SubTitle { get; set; }

    public string? Text { get; set; }

    public string? CssClasses { get; set; }

    public static CardNumber? Create(NestedBlockNumberCard numberCard, string? cssClasses = null)
    {
        if (numberCard.Title.IsNullOrWhiteSpace())
        {
            return null;
        }

        return new CardNumber
        {
            Title = numberCard.Title,
            SubTitle = numberCard.SubTitle,
            Text = numberCard.Text?.ToHtmlString(),
            CssClasses = cssClasses,
        };
    }
}
