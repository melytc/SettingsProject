using System.Windows;
using System.Windows.Controls;

#nullable enable

namespace Microsoft.VisualStudio.ProjectSystem.VS.Implementation.PropertyPages.Designer
{
    internal sealed class PropertyDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            if (item is Property property)
                return property.Editor?.PropertyDataTemplate;

            return null;
        }
    }
}