using Umbraco.Cms.Core.Models.PublishedContent;

namespace Umbraco.Cms.Web.Common.PublishedModels;

#pragma warning disable SA1402 // Suppress warning File may only contain a single type

public interface IBrandfolderAsset : IPublishedElement
{
    public string BrandfolderUrl { get; }

    public string Alt { get; }

    public string Name { get; }
}

public partial class BrandfolderImage : IBrandfolderAsset;

public partial class BrandfolderFile : IBrandfolderAsset;
