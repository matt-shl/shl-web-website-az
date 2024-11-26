using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class StackingCards
{
    private static readonly Dictionary<string, string> ThemeMapping = new()
    {
        { "t-dark-green", "t-pastel-green" },
        { "t-dark-pink", "t-lightest-pink" },
        { "t-general", "t-lightest-blue" },
        { "t-light-blue", "t-lightest-blue" },
        { "t-light-grey", "t-general" },
        { "t-lightest-blue", "t-general" },
        { "t-lightest-pink", "t-dark-pink" },
        { "t-lightest-yellow", "t-pale-yellow" },
        { "t-pale-blue", "t-general" },
        { "t-pale-green", "t-dark-green" },
        { "t-pale-pink", "t-dark-pink" },
        { "t-pale-yellow", "t-lightest-yellow" },
        { "t-pastel-blue", "t-general" },
        { "t-pastel-green", "t-dark-green" },
        { "t-white", "t-general" },
        { "t-white-pink", "t-dark-pink" },
    };

    public required string Title { get; set; }

    public required string Description { get; set; }

    public required List<StackingCardItem> Cards { get; set; }

    public required string ThemeOdd { get; set; }

    public required string ThemeEven { get; set; }

    public static StackingCards? Create(NestedBlockStackingCards stackingCardsBlock)
    {
        List<StackingCardItem> cards = stackingCardsBlock?.Cards?
            .Select(block => block.Content as NestedBlockStackingCard)
            .WhereNotNull()
            .Select(StackingCardItem.Create)
            .ToList() ?? [];

        if (cards.Count == 0)
        {
            return null;
        }

        string themeOdd = stackingCardsBlock?.FirstCardColor is not null ? $"t-{stackingCardsBlock?.FirstCardColor?.Label}" : "t-lightest-blue";
        string themeEven = ThemeMapping[themeOdd];

        return new StackingCards
        {
            Title = stackingCardsBlock?.Title!,
            Description = stackingCardsBlock?.Description!,
            Cards = cards,
            ThemeOdd = themeOdd,
            ThemeEven = themeEven,
        };
    }
}
