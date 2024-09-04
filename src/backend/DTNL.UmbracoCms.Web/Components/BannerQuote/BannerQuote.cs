using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class BannerQuote
{
    public required List<Quote> Quotes { get; set; }

    public static BannerQuote? Create(NestedBlockQuotesBanner? quotesBanner)
    {
        List<Quote> quotes = (quotesBanner?.Quotes)
            .Using(qb => qb.Content as NestedBlockQuote)
            .Using(q => new Quote
            {
                QuoteText = q.QuoteText!,
                Name = q.NameAuthor!,
                Company = q.Company,
                Role = q.Role,
                Image = Image.Create(q.Image)
                    .With(i =>
                    {
                        i.ImageStyle = "in-grid-banner-image";
                    }),
            })
            .ToList();

        if (quotes.Count == 0)
        {
            return null;
        }

        return new BannerQuote
        {
            Quotes = quotes,
        };
    }
}
