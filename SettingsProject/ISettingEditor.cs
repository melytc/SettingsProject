using System.Collections.Immutable;
using System.Windows;

#nullable enable

namespace SettingsProject
{
    internal interface ISettingEditor
    {
        string TypeName { get; }
        DataTemplate SettingDataTemplate { get; }
        DataTemplate? UnconfiguredDataTemplate { get; }
        DataTemplate? ConfiguredDataTemplate { get; }
        bool ShouldShowDescription(ImmutableArray<SettingValue> values);
    }
}