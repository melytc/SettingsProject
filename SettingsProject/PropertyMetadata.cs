using System;
using System.Collections.Immutable;
using System.ComponentModel;

#nullable enable

namespace Microsoft.VisualStudio.ProjectSystem.VS.Implementation.PropertyPages.Designer
{
    /// <summary>
    /// Immutable data about a property.
    /// </summary>
    internal sealed class PropertyMetadata
    {
        public string Name { get; }

        public string Page { get; }

        public string Category { get; }

        public string? Description { get; }

        /// <summary>
        /// Relative priority of the property, to use when ordering items in the UI.
        /// </summary>
        public int Priority { get; }

        /// <summary>
        /// Array of supported editors, in precedence order such that more desirable editors appear first in the array.
        /// </summary>
        public ImmutableArray<EditorSpecification> Editors { get; }

        public bool SupportsPerConfigurationValues { get; init; }

        public ImmutableArray<string> SearchTerms { get; init; } = ImmutableArray<string>.Empty;

        public PropertyMetadata([Localizable(true)] string name, [Localizable(true)] string? description, string page, string category, int priority, string editorType)
            : this(name, description, page, category, priority, ImmutableArray.Create(new EditorSpecification(editorType, ImmutableDictionary<string, string>.Empty)))
        {
        }

        public PropertyMetadata([Localizable(true)] string name, [Localizable(true)] string? description, string page, string category, int priority, ImmutableArray<EditorSpecification> editors)
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

        public PropertyIdentity Identity => new PropertyIdentity(Page, Category, Name);
    }
}