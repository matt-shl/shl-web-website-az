namespace Umbraco.Cms.Web.Common.PublishedModels;

#pragma warning disable SA1402 // Suppress warning File may only contain a single type

public interface IBlockSettings
{
    string? Identifier { get; }

    string? NavigationTitle { get; }
}

public partial class DefaultComponentSettings : IBlockSettings;

public partial class ColorComponentSettings : IBlockSettings;
