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
            new SettingsEditor("String", "GenericSettingTemplate", "UnconfiguredStringSettingValueTemplate", "ConfiguredStringSettingValueTemplate", _ => ""),
            new SettingsEditor("MultiLineString", "GenericSettingTemplate", "UnconfiguredMultilineStringSettingValueTemplate", "ConfiguredMultilineStringSettingValueTemplate", _ => ""),
            new SettingsEditor("Bool", "GenericSettingTemplate", "UnconfiguredBoolSettingValueTemplate", "ConfiguredBoolSettingValueTemplate", _ => false), // TODO box once
            new SettingsEditor("Enum", "GenericSettingTemplate", "UnconfiguredEnumSettingValueTemplate", "ConfiguredEnumSettingValueTemplate", metadata => metadata.EnumValues.FirstOrDefault() ?? ""),
            new SettingsEditor("LinkAction", "LinkActionTemplate", null, null, metadata => metadata.EnumValues.FirstOrDefault() ?? "")
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

        private sealed class SettingsEditor : ISettingEditor
        {
            private readonly Func<SettingMetadata, object> _defaultValue;
            public string TypeName { get; }
            public DataTemplate SettingDataTemplate { get; }
            public DataTemplate? UnconfiguredDataTemplate { get; }
            public DataTemplate? ConfiguredDataTemplate { get; }

            public object GetDefaultValue(SettingMetadata metadata)
            {
                return _defaultValue(metadata);
            }

            public SettingsEditor(string typeName, string settingDataTemplateName, string? unconfiguredDataTemplateName, string? configuredDataTemplateName, Func<SettingMetadata, object> defaultValue)
            {
                TypeName = typeName;
                _defaultValue = defaultValue;

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
        }
    }
}