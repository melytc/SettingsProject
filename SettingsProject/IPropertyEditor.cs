using System.Collections.Immutable;
using System.Windows;

#nullable enable

namespace Microsoft.VisualStudio.ProjectSystem.VS.Implementation.PropertyPages.Designer
{
    internal interface IPropertyEditor
    {
        string TypeName { get; }
        // TODO allow extenders to omit this, resulting in the default template being applied
        DataTemplate PropertyDataTemplate { get; }
        DataTemplate? UnconfiguredDataTemplate { get; }
        DataTemplate? ConfiguredDataTemplate { get; }
        bool ShouldShowDescription(ImmutableArray<PropertyValue> values);
    }
}