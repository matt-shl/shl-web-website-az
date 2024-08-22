using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class BannerQuote
{
    public string? AnchorId { get; set; }

    public string? AnchorTitle { get; set; }

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

        return new BannerQuote
        {
            AnchorId = quoteBanner.AnchorId,
            AnchorTitle = quoteBanner.AnchorTitle,
            Quotes = quoteBanner.Quotes?.Count > 0 ? quoteBanner.Quotes
            .Select(qb => qb.Content)
            .OfType<NestedBlockQuote>()
            .WhereNotNull()
            .Select(q => new Quote
            {
                Quotetext = q.QuoteText ?? "",
                Name = q.NameAuthor ?? "",
                Company = q.Company,
                Role = q.Role,
                Image = Image.Create(q.Image).With(i =>
                {
                    i.ImageStyle = "in-grid-banner-image";
                    i.ObjectFit = true;
                }),
            }).ToList() : [],
        };
    }
}
