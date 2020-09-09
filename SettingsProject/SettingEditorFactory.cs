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
            new SettingsEditor("String", "UnconfiguredStringSettingValueTemplate", "ConfiguredStringSettingValueTemplate"),
            new SettingsEditor("MultiLineString", "UnconfiguredMultilineStringSettingValueTemplate", "ConfiguredMultilineStringSettingValueTemplate"),
            new SettingsEditor("Bool", "UnconfiguredBoolSettingValueTemplate", "ConfiguredBoolSettingValueTemplate"),
            new SettingsEditor("Enum", "UnconfiguredEnumSettingValueTemplate", "ConfiguredEnumSettingValueTemplate"));

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
            public string TypeName { get; }
            public DataTemplate UnconfiguredDataTemplate { get; }
            public DataTemplate ConfiguredDataTemplate { get; }

            public SettingsEditor(string typeName, string unconfiguredDataTemplateName, string configuredDataTemplateName)
            {
                TypeName = typeName;

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