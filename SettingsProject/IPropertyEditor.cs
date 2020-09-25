using System.Collections.Immutable;
using System.Windows;

#nullable enable

namespace Microsoft.VisualStudio.ProjectSystem.VS.Implementation.PropertyPages.Designer
{
    internal interface IPropertyEditor
    {
        string TypeName { get; }
        DataTemplate PropertyDataTemplate { get; }
        DataTemplate? UnconfiguredDataTemplate { get; }
        DataTemplate? ConfiguredDataTemplate { get; }
        bool ShouldShowDescription(ImmutableArray<PropertyValue> values);
    }
}