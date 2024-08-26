using System.Diagnostics.CodeAnalysis;
using DTNL.UmbracoCms.Web.Helpers.Aliases;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Core.Dictionary;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class Button
{
    public string Element { get; set; } = "a";

    public required string Label { get; set; }

    public string? AriaLabel { get; set; }

    public bool LabelSrOnly { get; set; }

    public string? Url { get; set; }

    public string? Class { get; set; }

    public string? Variant { get; set; }

    public string? Size { get; set; }

    public string? Icon { get; set; }

    public string? IconPosition { get; set; }

    public string? Controls { get; set; }

    public Dictionary<string, string?> Attributes { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public string? Type { get; set; }

    public string? Hook { get; set; }

    public string? Target { get; set; }

    [return: NotNullIfNotNull(nameof(link))]
    public static Button? Create(Link? link)
    {
        return link is null ? null : new Button
        {
            Url = link.Url,
            Label = link.Label ?? "",
            Icon = link.IconPath,
            Target = link.Target,
        };
    }

    [return: NotNullIfNotNull(nameof(link))]
    public static Button? Create(Umbraco.Cms.Core.Models.Link? link)
    {
        return link is null ? null : new Button
        {
            Url = link.Url,
            Label = link.Name ?? "",
            Target = link.Target,
        };
    }

    public static Button? Create(BlockListItem? buttonLink, string? cssClasses = null, string? svgIcon = null, string? jsHook = null)
    {
        if (buttonLink?.Content is not NestedBlockButtonLink button)
        {
            return null;
        }

        return new Button()
        {
            Class = cssClasses,
            Icon = button.ButtonIcon?.LocalCrops.Src ?? svgIcon,
            Variant = button.Variant,
            Hook = jsHook,
            Label = button.Link?.Name ?? "",
        };
    }

    public static Button? CreateForEmail(string? email, string contactName, ICultureDictionary cultureDictionary)
    {
        if (email.IsNullOrWhiteSpace())
        {
            return null;
        }

        return new Button
        {
            Class = "card-contact__label",
            Variant = "link",
            Url = $"mailto:{email}",
            Label = email,
            AriaLabel = cultureDictionary.GetTranslation(TranslationAliases.Common.Cards.SendEmailTo, contactName),
        };
    }

    public static Button? CreateForPhoneNumber(string? phoneNumber, string contactName, ICultureDictionary cultureDictionary)
    {
        if (phoneNumber.IsNullOrWhiteSpace())
        {
            return null;
        }

        return new Button
        {
            Class = "card-contact__label",
            Variant = "link",
            Url = $"tel:{phoneNumber}",
            Label = phoneNumber,
            AriaLabel = cultureDictionary.GetTranslation(TranslationAliases.Common.Cards.MakePhoneCallTo, contactName),
        };
    }
}
