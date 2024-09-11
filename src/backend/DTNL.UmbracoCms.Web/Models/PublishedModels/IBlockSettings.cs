namespace Umbraco.Cms.Web.Common.PublishedModels;

public interface IBlockSettings
{
    string? Identifier { get; }

    string? NavigationTitle { get; }
}

public partial class DefaultComponentSettings : IBlockSettings;

public partial class ColorComponentSettings : IBlockSettings;
