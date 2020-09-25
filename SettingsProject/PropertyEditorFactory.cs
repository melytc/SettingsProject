using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Windows;

#nullable enable

namespace Microsoft.VisualStudio.ProjectSystem.VS.Implementation.PropertyPages.Designer
{
    internal sealed class PropertyEditorFactory
    {
        public static PropertyEditorFactory Default { get; } = new PropertyEditorFactory(
            new StringPropertyEditor(),
            new MultiLineStringPropertyEditor(),
            new BoolPropertyEditor(),
            new EnumPropertyEditor(),
            new FileBrowseEditor(),
            new LinkActionEditor()
        );

        private readonly Dictionary<string, IPropertyEditor> _editorByTypeName;

        private PropertyEditorFactory(params IPropertyEditor[] editors)
        {
            _editorByTypeName = editors.ToDictionary(editor => editor.TypeName);
        }

        public (IPropertyEditor?, IReadOnlyDictionary<string, string>) GetEditor(ImmutableArray<EditorSpecification> editorSpecifications)
        {
            foreach (var editorSpecification in editorSpecifications)
            {
                if (_editorByTypeName.TryGetValue(editorSpecification.TypeName, out var editor))
                {
                    return (editor, editorSpecification.Metadata);
                }
            }

            return (null, ImmutableDictionary<string, string>.Empty);
        }

        private abstract class PropertyEditorBase : IPropertyEditor
        {
            public string TypeName { get; }
            public DataTemplate PropertyDataTemplate { get; }
            public DataTemplate? UnconfiguredDataTemplate { get; }
            public DataTemplate? ConfiguredDataTemplate { get; }

            protected PropertyEditorBase(string typeName, string propertyDataTemplateName, string? unconfiguredDataTemplateName, string? configuredDataTemplateName)
            {
                TypeName = typeName;

                var propertyDataTemplate = (DataTemplate?)Application.Current.FindResource(propertyDataTemplateName);

                Assumes.NotNull(propertyDataTemplate);

                PropertyDataTemplate = propertyDataTemplate;

                if (unconfiguredDataTemplateName != null)
                {
                    UnconfiguredDataTemplate = (DataTemplate?)Application.Current.FindResource(unconfiguredDataTemplateName);
                }

                if (configuredDataTemplateName != null)
                {
                    ConfiguredDataTemplate = (DataTemplate?)Application.Current.FindResource(configuredDataTemplateName);
                }
            }

            public virtual bool ShouldShowDescription(ImmutableArray<PropertyValue> values) => true;
        }

        private sealed class StringPropertyEditor : PropertyEditorBase
        {
            public StringPropertyEditor() : base("String", "GenericPropertyTemplate", "UnconfiguredStringPropertyValueTemplate", "ConfiguredStringPropertyValueTemplate") {}
        }

        private sealed class MultiLineStringPropertyEditor : PropertyEditorBase
        {
            public MultiLineStringPropertyEditor() : base("MultiLineString", "GenericPropertyTemplate", "UnconfiguredMultilineStringPropertyValueTemplate", "ConfiguredMultilineStringPropertyValueTemplate") {}
        }

        private sealed class BoolPropertyEditor : PropertyEditorBase
        {
            public BoolPropertyEditor() : base("Bool", "GenericPropertyTemplate", "UnconfiguredBoolPropertyValueTemplate", "ConfiguredBoolPropertyValueTemplate") {}
            public override bool ShouldShowDescription(ImmutableArray<PropertyValue> values) => values.Length > 1;
        }

        private sealed class EnumPropertyEditor : PropertyEditorBase
        {
            public EnumPropertyEditor() : base("Enum", "GenericPropertyTemplate", "UnconfiguredEnumPropertyValueTemplate", "ConfiguredEnumPropertyValueTemplate") {}
        }

        private sealed class FileBrowseEditor : PropertyEditorBase
        {
            public FileBrowseEditor() : base("FileBrowse", "GenericPropertyTemplate", "UnconfiguredFileBrowsePropertyValueTemplate", "ConfiguredFileBrowsePropertyValueTemplate") {}
        }

        private sealed class LinkActionEditor : PropertyEditorBase
        {
            public LinkActionEditor() : base("LinkAction", "LinkActionTemplate", null, null) {}
        }
    }
}