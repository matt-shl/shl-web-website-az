using DTNL.UmbracoCms.Web.Models.Filters;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Models.Products;

public class ProductFilters : Dictionary<string, FilterOption[]>
{
    public static readonly (string Name, Func<PageProduct, IEnumerable<string>?> GetValues)[] FilterFields =
    [
        (nameof(PageProduct.DeviceType), p => p.DeviceType),
        (nameof(PageProduct.ViscosityLevel), p => p.ViscosityLevel.IsNullOrWhiteSpace() ? null : [p.ViscosityLevel]),
        (nameof(PageProduct.VolumeRange), p => p.VolumeRange),
        (nameof(PageProduct.ContainerType), p => p.ContainerType),
        (nameof(PageProduct.RouteOfAdministration), p => p.RouteOfAdministration),
        (nameof(PageProduct.ConnectivityType), p => p.ConnectivityType),
    ];

    public bool IsSelected(string name, FilterOption option)
    {
        return TryGetValue(name, out FilterOption[]? filterOptions) && filterOptions.Contains(option);
    }
}
