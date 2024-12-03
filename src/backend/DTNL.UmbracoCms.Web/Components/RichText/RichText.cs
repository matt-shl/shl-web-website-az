using DTNL.UmbracoCms.Web.Helpers.Aliases;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Core.Dictionary;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class RichText
{
    public string? Content { get; set; }

    public Button? PrimaryLinkButton { get; set; }

    public Button? SecondaryLinkButton { get; set; }

    public Button? ReadMoreButton { get; set; }

    public Button? ReadLessButton { get; set; }

    public string? TextSize { get; set; }

    public bool ReadMoreOption { get; set; }

    public static RichText Create(PageVacancy pageVacancy, string applyLinkLabel)
    {
        return new RichText
        {
            Content = pageVacancy.JobDescription?.ToHtmlString(),
            PrimaryLinkButton = new Button
            {
                Label = applyLinkLabel,
                Url = pageVacancy.ExternalUrl,
                Target = "_blank",
                Variant = "secondary",
                Icon = SvgAliases.Icons.ArrowTopRight,
                Hook = "apply",
            },
        };
    }

    public static RichText Create(NestedBlockRichText richTextBlock, ICultureDictionary cultureDictionary)
    {
        return new RichText
        {
            Content = richTextBlock.Text?.ToHtmlString(),
            TextSize = richTextBlock.TextSize,
            PrimaryLinkButton = Button
                .Create(richTextBlock.PrimaryLink, fallBackVariant: "primary")
                .With(b =>
                {
                    b.Class = "rich-text__cta1";
                    b.Hook = "js-hook-rich-text-button";
                }),
            SecondaryLinkButton = Button
                .Create(richTextBlock.SecondLink, fallBackVariant: "secondary")
                .With(b =>
                {
                    b.Class = "rich-text__cta2";
                    b.Hook = "js-hook-rich-text-button";
                }),
            ReadMoreOption = richTextBlock.ShowReadMoreOption,
            ReadMoreButton = new Button
            {
                Url = "#",
                Element = "button",
                Label = $"{cultureDictionary.GetTranslation(TranslationAliases.Common.RichText.ReadMore)}",
                Class = "rich-text__button rich-text__button-more",
                Variant = "link-underlined",
                Hook = "rich-text-button",
                Attributes = new Dictionary<string, string?>
                {
                    ["aria-hidden"] = "true",
                },
            },
            ReadLessButton = new Button
            {
                Url = "#",
                Label = cultureDictionary.GetTranslation(TranslationAliases.Common.RichText.ReadLess),
                Element = "button",
                Class = "rich-text__button rich-text__button-less",
                Variant = "link-underlined",
                Hook = "rich-text-button",
                Attributes = new Dictionary<string, string?>
                {
                    ["aria-hidden"] = "true",
                },
            },
        };
    }
}
