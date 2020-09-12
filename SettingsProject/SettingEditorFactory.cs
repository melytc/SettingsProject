using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Windows;
using Microsoft;

#nullable enable

namespace SettingsProject
{
    internal sealed class SettingEditorFactory
    {
        public static SettingEditorFactory Default { get; } = new SettingEditorFactory(
            new StringSettingEditor(),
            new MultiLineStringSettingEditor(),
            new BoolSettingEditor(),
            new EnumSettingEditor(),
            new FileBrowseEditor(),
            new LinkActionEditor()
        );

        private readonly Dictionary<string, ISettingEditor> _editorByTypeName;

        private SettingEditorFactory(params ISettingEditor[] editors)
        {
            _editorByTypeName = editors.ToDictionary(editor => editor.TypeName);
        }

        public ISettingEditor GetEditor(string typeName)
        {
            return _editorByTypeName[typeName];
        }

        private abstract class SettingEditorBase : ISettingEditor
        {
            public string TypeName { get; }
            public DataTemplate SettingDataTemplate { get; }
            public DataTemplate? UnconfiguredDataTemplate { get; }
            public DataTemplate? ConfiguredDataTemplate { get; }

            protected SettingEditorBase(string typeName, string settingDataTemplateName, string? unconfiguredDataTemplateName, string? configuredDataTemplateName)
            {
                TypeName = typeName;

                var settingDataTemplate = (DataTemplate?)Application.Current.FindResource(settingDataTemplateName);

                Assumes.NotNull(settingDataTemplate);

                SettingDataTemplate = settingDataTemplate;

                if (unconfiguredDataTemplateName != null)
                {
                    UnconfiguredDataTemplate = (DataTemplate?)Application.Current.FindResource(unconfiguredDataTemplateName);
                }

                if (configuredDataTemplateName != null)
                {
                    ConfiguredDataTemplate = (DataTemplate?)Application.Current.FindResource(configuredDataTemplateName);
                }
            }

            public abstract object GetDefaultValue(SettingMetadata metadata);

            public virtual bool ShouldShowDescription(ImmutableArray<SettingValue> values) => true;
        }

        private sealed class StringSettingEditor : SettingEditorBase
        {
            public StringSettingEditor() : base("String", "GenericSettingTemplate", "UnconfiguredStringSettingValueTemplate", "ConfiguredStringSettingValueTemplate") {}
            public override object GetDefaultValue(SettingMetadata metadata) => "";
        }

        private sealed class MultiLineStringSettingEditor : SettingEditorBase
        {
            public MultiLineStringSettingEditor() : base("MultiLineString", "GenericSettingTemplate", "UnconfiguredMultilineStringSettingValueTemplate", "ConfiguredMultilineStringSettingValueTemplate") {}
            public override object GetDefaultValue(SettingMetadata metadata) => "";
        }

        private sealed class BoolSettingEditor : SettingEditorBase
        {
            private static readonly object BoxedFalse = false;
            public BoolSettingEditor() : base("Bool", "GenericSettingTemplate", "UnconfiguredBoolSettingValueTemplate", "ConfiguredBoolSettingValueTemplate") {}
            public override object GetDefaultValue(SettingMetadata metadata) => BoxedFalse;
            public override bool ShouldShowDescription(ImmutableArray<SettingValue> values) => values.Length > 1;
        }

        private sealed class EnumSettingEditor : SettingEditorBase
        {
            public EnumSettingEditor() : base("Enum", "GenericSettingTemplate", "UnconfiguredEnumSettingValueTemplate", "ConfiguredEnumSettingValueTemplate") {}
            public override object GetDefaultValue(SettingMetadata metadata) => metadata.EnumValues.FirstOrDefault() ?? "";
        }

        private sealed class FileBrowseEditor : SettingEditorBase
        {
            public FileBrowseEditor() : base("FileBrowse", "GenericSettingTemplate", "UnconfiguredFileBrowseSettingValueTemplate", "ConfiguredFileBrowseSettingValueTemplate") {}
            public override object GetDefaultValue(SettingMetadata metadata) => "";
        }

        private sealed class LinkActionEditor : SettingEditorBase
        {
            public LinkActionEditor() : base("LinkAction", "LinkActionTemplate", null, null) {}
            public override object GetDefaultValue(SettingMetadata metadata) => "";
        }
    }
}