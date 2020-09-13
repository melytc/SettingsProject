using System.Collections.Generic;
using System.Collections.Immutable;

#nullable enable

namespace System.Runtime.CompilerServices
{
    internal class IsExternalInit
    {
    }
}

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

        // TODO will probably be an array in precedence order
        public string EditorType { get; }

        public IReadOnlyDictionary<string, string> EditorMetadata { get; init; } = ImmutableDictionary<string, string>.Empty;

        public bool SupportsPerConfigurationValues { get; init; } = false;

        public ISettingEditor? Editor { get; }

        public ImmutableArray<string> SearchTerms { get; init; } = ImmutableArray<string>.Empty;

        public SettingMetadata(string name, string? description, string page, string category, int priority, string editorType)
        {
            Name = name;
            Page = page;
            Category = category;
            Description = description;
            Priority = priority;
            EditorType = editorType;

            Editor = SettingEditorFactory.Default.GetEditor(EditorType);
        }

        public SettingIdentity Identity => new SettingIdentity(Page, Category, Name);
    }
}
