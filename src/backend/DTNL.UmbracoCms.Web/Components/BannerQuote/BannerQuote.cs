using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class BannerQuote
{
    public string? Theme { get; set; }

    public required List<Quote> Quotes { get; set; } = new List<Quote>();

    public static BannerQuote? Create(NestedBlockQuoteBanner? quoteBanner)
    {
        if (quoteBanner is null)
        {
            return null;
        }

        if (!(quoteBanner.Quotes?.Count > 0))
        {
            return null;
        }

        var ssss = quoteBanner?.Quotes?.Count > 0 ? quoteBanner.Quotes
            .Select(qb => qb.Content)
            .OfType<NestedBlockQuote>()
            .WhereNotNull()
            .Select(q => new Quote
            {
                Quotetext = q.QuoteText ?? "",
                Name = q.NameAuthor ?? "",
                Company = q.Company,
                Role = q.Role,
                Image = q.Image,
            }).ToList() : [];

        return new BannerQuote
        {
            Quotes = quoteBanner?.Quotes?.Count > 0 ? quoteBanner.Quotes
            .Select(qb => qb.Content)
            .OfType<NestedBlockQuote>()
            .WhereNotNull()
            .Select(q => new Quote
            {
                Quotetext = q.QuoteText ?? "",
                Name = q.NameAuthor ?? "",
                Company = q.Company,
                Role = q.Role,
                Image = q.Image,
            }).ToList() : [],
        };
    }
}
