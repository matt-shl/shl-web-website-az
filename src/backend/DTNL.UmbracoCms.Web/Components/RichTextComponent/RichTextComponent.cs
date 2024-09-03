using DTNL.UmbracoCms.Web.Helpers.Aliases;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Core.Dictionary;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class RichTextComponent : LayoutSection
{
    public string? Content { get; set; }

    public Button? FirstButton { get; set; }

    public Button? SecondButton { get; set; }

    public Button? ReadMoreButton { get; set; }

    public string? SmallTextClass { get; set; }

    public string? ReadMoreTextClass { get; set; }

    public bool ShouldHaveReadMore { get; set; }

    public static RichTextComponent Create(NestedBlockRichTextComponent richTextComponent, ICultureDictionary cultureDictionary)
    {
        NestedBlockButtonLink? firstButton = richTextComponent.FirstButton?.FirstOrDefault()?.Content as NestedBlockButtonLink;
        NestedBlockButtonLink? secondButton = richTextComponent.FirstButton?.FirstOrDefault()?.Content as NestedBlockButtonLink;

        return new RichTextComponent
        {
            ShouldHaveReadMore = richTextComponent.ShouldHaveReadMore,

            Content = richTextComponent.RTecontent?.ToHtmlString(),

            FirstButton = Button.Create(firstButton?.Link).With(b =>
            {
                b.Label = TranslationAliases.Common.Richtextcomponent.Readmore;
                b.Hook = "js-hook-rich-text-button";
                b.Icon = firstButton?.ButtonIcon?.LocalCrops.Src ?? SvgAliases.Icons.ArrowTopRight;
            }),

            SecondButton = Button.Create(secondButton?.Link).With(b =>
            {
                b.Label = TranslationAliases.Common.Richtextcomponent.Readless;
                b.Hook = "js-hook-rich-text-button";
                b.Variant = "secondary";
                b.Icon = secondButton?.ButtonIcon?.LocalCrops.Src ?? SvgAliases.Icons.ArrowTopRight;
            }),

            ReadMoreButton = Button.Create(Link.Create(richTextComponent as IPublishedContent).With(l =>
            {
                l.Url = "#";
                l.Label = cultureDictionary.GetTranslation(TranslationAliases.Common.Richtextcomponent.Readmore);
            }
            )),

            ReadMoreTextClass = richTextComponent.ShouldHaveReadMore ? "c-rich-text--is-closed" : null,

            SmallTextClass = richTextComponent.SmallText ? "c-rich-text--size-small" : "c-rich-text--size-large",

        };
    }
}
