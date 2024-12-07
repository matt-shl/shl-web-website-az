using System.Globalization;
using System.Text.Json;
using System.Xml.Linq;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using DTNL.UmbracoCms.Web.Infrastructure.DependencyInjection;
using HtmlAgilityPack;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Helpers;

[Transient]
public class EventsContentHelper
{
    private static readonly XNamespace ContentNs = "http://purl.org/rss/1.0/modules/content/";

    private static readonly string[] ContentTypeAliases = [NestedBlockContentHero.ModelTypeAlias, NestedBlockRichText.ModelTypeAlias];

    private readonly List<IContentType> _contentTypes;

    public EventsContentHelper(IContentTypeService contentTypeService)
    {
        _contentTypes = contentTypeService
            .GetAll()
            .Where(c => ContentTypeAliases.Contains(c.Alias))
            .ToList();
    }

    public void SetContent(IContent pageContent, XElement post, IEnumerable<string> cultures)
    {
        foreach (string culture in cultures)
        {
            SetContent(pageContent, post, culture);
        }
    }

    public void SetContent(
        IContent pageContent,
        XElement post,
        string culture)
    {
        pageContent.SetCultureName(pageContent.Name, culture);

        pageContent.SetValue<PageEvent>(p => p.Date, GetDateOrNull((string?) post.Element("pubDate")), culture);

        XElement? category = post.Elements("category")
            .FirstOrDefault(c => c.Attribute("domain")?.Value == "post_tag");

        if (!string.IsNullOrWhiteSpace(category?.Value))
        {
            pageContent.SetValue<PageEvent>(p => p.ContentTags, JsonSerializer.Serialize(new[] { category.Value }), culture);
        }

        SetHeroContent(pageContent, post, culture);

        SetRichTextContent(pageContent, post, culture);
    }

    private void SetHeroContent(IContent pageContent, XElement post, string culture)
    {
        var blockContent = new
        {
            title = (string?) post.Element("title"),
        };

        string json = BlockListCreatorService
            .GetBlockListJsonFor([blockContent], _contentTypes.First(c => c.Alias == NestedBlockContentHero.ModelTypeAlias).Key);

        pageContent.SetValue<PageVacancy>(x => x.Hero, json, culture);
    }

    private void SetRichTextContent(IContent pageContent, XElement post, string culture)
    {
        string? content = (string?) post.Element(ContentNs + "encoded");

        if (string.IsNullOrWhiteSpace(content))
        {
            return;
        }

        HtmlDocument htmlDoc = new();

        htmlDoc.LoadHtml(content);

        if (htmlDoc.DocumentNode.SelectNodes("//figure") is { } figureNodes)
        {
            foreach (HtmlNode figure in figureNodes)
            {
                figure.Remove();
            }
        }

        if (htmlDoc.DocumentNode.SelectNodes("//img") is { } imgNodes)
        {
            foreach (HtmlNode img in imgNodes)
            {
                img.Remove();
            }
        }

        if (htmlDoc.DocumentNode.SelectNodes("//comment()") is { } commentNodes)
        {
            foreach (HtmlNode comment in commentNodes)
            {
                comment.Remove();
            }
        }

        content = htmlDoc.DocumentNode.InnerHtml;

        var blockContentHero = new
        {
            text = content,
        };

        string json = BlockListCreatorService
            .GetBlockListJsonFor([blockContentHero], _contentTypes.First(c => c.Alias == NestedBlockRichText.ModelTypeAlias).Key);

        pageContent.SetValue<PageVacancy>(x => x.ContentBlocks, json, culture);
    }

    private static DateTime? GetDateOrNull(string? dateString)
    {
        const string dateFormat = "ddd, dd MMM yyyy HH:mm:ss zzz";

        if (DateTime
            .TryParseExact(
                dateString,
                dateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal,
                out DateTime date))
        {
            return date;
        }

        return null;
    }
}
