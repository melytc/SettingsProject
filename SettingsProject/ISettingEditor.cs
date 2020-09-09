using System.Windows;

#nullable enable

namespace SettingsProject
{
    internal interface ISettingEditor
    {
        string TypeName { get; }
        DataTemplate UnconfiguredDataTemplate { get; }
        DataTemplate ConfiguredDataTemplate { get; }
    }
}