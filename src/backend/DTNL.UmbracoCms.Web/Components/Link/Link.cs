using System.Diagnostics.CodeAnalysis;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace DTNL.UmbracoCms.Web.Components;

public class Link
{
    /// <summary>
    /// Gets or sets the link label.
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Gets or sets the url of the link.
    /// </summary>
    public required string Url { get; set; }

    /// <summary>
    /// Gets or sets the target of the link.
    /// </summary>
    public string? Target { get; set; }

    /// <summary>
    /// Gets or sets the list of additional HTML attributes that will be added to the link.
    /// </summary>
    public Dictionary<string, string?> Attributes { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Gets or sets the list of additional CSS classes that will be added to the link.
    /// </summary>
    public string? CssClasses { get; set; }

    /// <summary>
    /// Gets or sets the icon path of the link.
    /// </summary>
    public string? IconPath { get; set; }

    public static Link? Create(IPublishedContent? node, bool showTitle = true, string? cssClasses = null)
    {
        if (node == null)
        {
            return null;
        }

        string? label = node.Name;
        if (showTitle && node.GetTitle() is { Length: > 0 } title)
        {
            label = title;
        }

        return new Link
        {
            Url = node.Url(),
            Label = label,
            CssClasses = cssClasses,
        };
    }

    [return: NotNullIfNotNull(nameof(link))]
    public static Link? Create(Umbraco.Cms.Core.Models.Link? link, string? cssClasses = null, string? icon = null, bool hideLabel = false)
    {
        if (link == null)
        {
            return null;
        }

        return new Link
        {
            Url = link.Url ?? "",
            Target = link.Target,
            Label = !hideLabel ? link.Name : null,
            CssClasses = cssClasses,
            IconPath = icon,
        };
    }
}
