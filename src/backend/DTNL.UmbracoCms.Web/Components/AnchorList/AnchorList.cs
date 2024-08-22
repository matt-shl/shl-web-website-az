using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class AnchorList
{
    public string? Alignment { get; set; }

    public string? CssClasses { get; set; }

    public bool IsComponent { get; set; }

    public required List<Link> Links { get; set; }

    public Button? LinkButton { get; set; }

    public class Anchor
    {
        public required string AnchorId { get; set; }

        public required string AnchorTitle { get; set; }
    }

    public static AnchorList Create(ICompositionContentBlocks? block = null)
    {
        List<Link>? links = block?.ContentBlocks
                        ?.Select(b => b.Content as ICompositionAnchors)
                        .Select(b => new Link { Label = b?.AnchorTitle, Url = $"#{b?.AnchorId}" })
                        .ToList();
        return new AnchorList
        {
            IsComponent = block is null,
            Links = links ?? [],
        };
    }
}
