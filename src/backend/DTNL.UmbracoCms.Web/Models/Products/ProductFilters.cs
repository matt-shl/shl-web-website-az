using DTNL.UmbracoCms.Web.Models.Filters;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Models.Products;

public class ProductFilters : BaseFilters
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

    public ProductFilters(IPublishedContent overviewPage, IQueryCollection queryCollection) : base(overviewPage, queryCollection)
    { }
}
