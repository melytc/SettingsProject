using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft;

#nullable enable

namespace SettingsProject
{
    internal sealed class SettingEditorFactory
    {
        public static SettingEditorFactory Default { get; } = new SettingEditorFactory(
            new SettingsEditor("String", "UnconfiguredStringSettingValueTemplate", "ConfiguredStringSettingValueTemplate", _ => ""),
            new SettingsEditor("MultiLineString", "UnconfiguredMultilineStringSettingValueTemplate", "ConfiguredMultilineStringSettingValueTemplate", _ => ""),
            new SettingsEditor("Bool", "UnconfiguredBoolSettingValueTemplate", "ConfiguredBoolSettingValueTemplate", _ => false), // TODO box once
            new SettingsEditor("Enum", "UnconfiguredEnumSettingValueTemplate", "ConfiguredEnumSettingValueTemplate", metadata => metadata.EnumValues.FirstOrDefault() ?? ""));

        private readonly Dictionary<string, ISettingEditor> _editorByTypeName;

        private SettingEditorFactory(params ISettingEditor[] editors)
        {
            _editorByTypeName = editors.ToDictionary(editor => editor.TypeName);
        }

        public ISettingEditor GetEditor(string typeName)
        {
            return _editorByTypeName[typeName];
        }

        private sealed class SettingsEditor : ISettingEditor
        {
            private readonly Func<SettingMetadata, object> _defaultValue;
            public string TypeName { get; }
            public DataTemplate UnconfiguredDataTemplate { get; }
            public DataTemplate ConfiguredDataTemplate { get; }

            public object GetDefaultValue(SettingMetadata metadata)
            {
                return _defaultValue(metadata);
            }

            public SettingsEditor(string typeName, string unconfiguredDataTemplateName, string configuredDataTemplateName, Func<SettingMetadata, object> defaultValue)
            {
                TypeName = typeName;
                _defaultValue = defaultValue;

                var unconfiguredDataTemplate = (DataTemplate?)Application.Current.FindResource(unconfiguredDataTemplateName);
                var configuredDataTemplate = (DataTemplate?)Application.Current.FindResource(configuredDataTemplateName);

                Assumes.NotNull(unconfiguredDataTemplate);
                Assumes.NotNull(configuredDataTemplate);

                UnconfiguredDataTemplate = unconfiguredDataTemplate;
                ConfiguredDataTemplate = configuredDataTemplate;
            }
        }
    }
}