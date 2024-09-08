using Umbraco.Cms.Core.PropertyEditors.ValueConverters;

namespace Umbraco.Cms.Web.Common.PublishedModels;

public interface IPageTheme
{
    ColorPickerValueConverter.PickedColor? PageTheme { get; }
}

public partial interface ICompositionColorOptions : IPageTheme;

public partial interface ICompositionColorOptionsPageHome : IPageTheme;
