using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.WebAssets;
using Umbraco.Cms.Infrastructure.WebAssets;

namespace DTNL.UmbracoCms.Web.Modules.ReadOnlyTagsEditor.App_Plugins;

#pragma warning disable SA1402 // Suppress warning File may only contain a single type

[TagsPropertyEditor]
[DataEditor(EditorAlias, EditorType.PropertyValue, "Tags - Read Only", Folder + "ReadOnlyTagsEditor.html", Icon = "icon-tags", ValueType = ValueTypes.Json)]
[PropertyEditorAsset(AssetType.Javascript, Folder + "read-only-tags.html")]
public class ReadOnlyTags : TagsPropertyEditor
{
    public const string EditorAlias = "readOnlyTags";
    public const string Folder = "/App_Plugins/ReadOnlyTagsEditor/";

    public ReadOnlyTags(
        IDataValueEditorFactory dataValueEditorFactory,
        ManifestValueValidatorCollection validators,
        IIOHelper ioHelper,
        ILocalizedTextService localizedTextService,
        IEditorConfigurationParser editorConfigurationParser,
        ITagPropertyIndexValueFactory tagPropertyIndexValueFactory)
        : base(dataValueEditorFactory, validators, ioHelper, localizedTextService, editorConfigurationParser, tagPropertyIndexValueFactory)
    {
    }
}

public class ReadOnlyTagsPropertyConverter : TagsValueConverter
{
    public ReadOnlyTagsPropertyConverter(IJsonSerializer jsonSerializer)
        : base(jsonSerializer)
    {
    }

    public override bool IsConverter(IPublishedPropertyType propertyType)
    {
        return propertyType.EditorAlias.InvariantEquals(ReadOnlyTags.EditorAlias);
    }
}
