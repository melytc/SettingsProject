using System.Windows;
using System.Windows.Controls;

#nullable enable

namespace SettingsProject
{
    internal sealed class SettingDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            if (item is Setting setting)
                return setting.Metadata.Editor?.SettingDataTemplate;

            return null;
        }
    }
}