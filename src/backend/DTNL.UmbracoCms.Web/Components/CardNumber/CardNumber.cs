using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class CardNumber
{
    public required string Title { get; set; }
    public string? SubTitle { get; set; }

    public string? Text { get; set; }

    public string? ThemeCssClass { get; set; }

    public CardNumber? Create(NestedBlockNumberCard numberCard, string? themeCssClass)
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
            ThemeCssClass = themeCssClass,
        };
    }
}
