using System.Windows;
using System.Windows.Controls;

#nullable enable

namespace SettingsProject
{
    internal sealed class SettingDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            if (item is LinkAction)
                return LinkActionTemplate;

            return SettingTemplate;
        }

        public DataTemplate? SettingTemplate { get; set; }
        public DataTemplate? LinkActionTemplate { get; set; }
    }
}