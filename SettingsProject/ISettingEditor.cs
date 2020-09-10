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
        object GetDefaultValue(SettingMetadata metadata);
        bool ShouldShowDescription(ImmutableArray<ISettingValue> values);
    }
}