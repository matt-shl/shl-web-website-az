using DTNL.UmbracoCms.Web.Helpers.Aliases;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Core.Dictionary;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class RichTextComponent : LayoutSection
{
    public string? Content { get; set; }

    public Button? FirstButton { get; set; }

    public Button? SecondButton { get; set; }

    public Button? ReadMoreButton { get; set; }

    public Button? ReadLessButton { get; set; }

    public string? TextSize { get; set; }

    public string? ReadMoreOptionClass { get; set; }

    public bool ReadMoreOption { get; set; }

    public static RichTextComponent Create(NestedBlockRichTextComponent richTextComponent, ICultureDictionary cultureDictionary)
    {
        NestedBlockButtonLink? firstButton = richTextComponent.FirstButton?.FirstOrDefault()?.Content as NestedBlockButtonLink;
        NestedBlockButtonLink? secondButton = richTextComponent.SecondButton?.FirstOrDefault()?.Content as NestedBlockButtonLink;

        return new RichTextComponent
        {
            Content = richTextComponent.RTecontent?.ToHtmlString(),
            TextSize = !string.IsNullOrWhiteSpace(richTextComponent.TextSize) ? $"c-rich-text--size-{richTextComponent.TextSize}" : null,
            FirstButton = Button.Create(firstButton).With(b =>
            {
                b.Class = "rich-text__cta1";
                b.Label = firstButton?.Link?.Name ?? "";
                b.Variant = firstButton?.Variant ?? "primary";
                b.Hook = "js-hook-rich-text-button";
                b.Icon = firstButton?.ButtonIcon?.LocalCrops.Src ?? SvgAliases.Icons.ArrowTopRight;
            }),
            SecondButton = Button.Create(secondButton).With(b =>
            {
                b.Class = "rich-text__cta2";
                b.Label = secondButton?.Link?.Name ?? "";
                b.Hook = "js-hook-rich-text-button";
                b.Variant = secondButton?.Variant ?? "secondary";
                b.Icon = secondButton?.ButtonIcon?.LocalCrops.Src ?? SvgAliases.Icons.ArrowTopRight;
            }),
            ReadMoreOption = richTextComponent.ReadMorelessOption,
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
