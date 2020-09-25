using System.Windows;
using System.Windows.Controls;

#nullable enable

namespace Microsoft.VisualStudio.ProjectSystem.VS.Implementation.PropertyPages.Designer
{
    internal sealed class SettingDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            if (item is Setting setting)
                return setting.Editor?.SettingDataTemplate;

            return null;
        }
    }
}