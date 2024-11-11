using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public partial class Map
{
    public string? Title { get; set; }

    public required List<Region> Regions { get; set; }

    public static Map Create(NestedBlockMap block)
    {
        return new Map
        {
            Title = block.Title,
            Regions = block.Regions
                .Using(r => r.Content as NestedBlockMapRegion)
                .Using(Region.Create)
                .ToList(),
        };
    }
}
