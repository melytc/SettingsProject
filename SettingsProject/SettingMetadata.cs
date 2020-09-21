using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;

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

        /// <summary>
        /// Array of supported editors, in precedence order such that more desirable editors appear first in the array.
        /// </summary>
        public ImmutableArray<EditorSpecification> Editors { get; }

        public bool SupportsPerConfigurationValues { get; init; }

        public ImmutableArray<string> SearchTerms { get; init; } = ImmutableArray<string>.Empty;

        public SettingMetadata([Localizable(true)] string name, [Localizable(true)] string? description, string page, string category, int priority, string editorType)
            : this(name, description, page, category, priority, ImmutableArray.Create(new EditorSpecification(editorType, ImmutableDictionary<string, string>.Empty)))
        {
        }

        public SettingMetadata([Localizable(true)] string name, [Localizable(true)] string? description, string page, string category, int priority, ImmutableArray<EditorSpecification> editors)
        {
            if (editors.IsEmpty)
                throw new ArgumentException("Cannot be empty.", nameof(editors));

            Name = name;
            Page = page;
            Category = category;
            Description = description;
            Priority = priority;
            Editors = editors;
        }

        public SettingIdentity Identity => new SettingIdentity(Page, Category, Name);
    }

    internal sealed class EditorSpecification
    {
        public string TypeName { get; }

        public IReadOnlyDictionary<string, string> Metadata { get; }

        public EditorSpecification(string typeName, IReadOnlyDictionary<string, string> metadata)
        {
            TypeName = typeName;
            Metadata = metadata;
        }
    }
}
