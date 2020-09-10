using System.Collections.Immutable;

#nullable enable

namespace SettingsProject
{
    /// <summary>
    /// Immutable data about a setting.
    /// </summary>
    internal sealed class SettingMetadata
    {
        public string Name { get; }

        public string Page { get; }

        public string Category { get; }

        public string? Description { get; }

        /// <summary>
        /// Relative priority of the setting, to use when ordering items in the UI.
        /// </summary>
        public int Priority { get; }

        // TODO don't allow this to be null -- it's currently null only for LinkAction, which could have its own editor type if that specified the top-level template too
        // TODO will probably be an array in precedence order
        public string? EditorType { get; }

        public bool SupportsPerConfigurationValues { get; }

        // TODO this will move to the SettingValue type probably
        public ImmutableArray<string> EnumValues { get; }

        public ISettingEditor? Editor { get; }

        public SettingMetadata(string name, string page, string category, string? description, int priority, string? editorType, bool supportsPerConfigurationValues, ImmutableArray<string> enumValues)
        {
            Name = name;
            Page = page;
            Category = category;
            Description = description;
            Priority = priority;
            EditorType = editorType;
            SupportsPerConfigurationValues = supportsPerConfigurationValues;
            EnumValues = enumValues;

            if (EditorType != null)
            {
                Editor = SettingEditorFactory.Default.GetEditor(EditorType);
            }
        }

        public SettingIdentity Identity => new SettingIdentity(Page, Category, Name);
    }
}
