using System.Collections.Immutable;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

#nullable enable

namespace SettingsProject
{
    internal sealed class SettingValue : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private object _value;
        private ImmutableArray<string> _enumValues = ImmutableArray<string>.Empty;

        public SettingValue(ImmutableDictionary<string, string> configurationDimensions, object value)
        {
            ConfigurationDimensions = configurationDimensions;
            _value = value;
        }

        // the set of dimensions for which this value is specified. any omitted dimensions are invariant.
        // empty if this value applies to all configurations
        public ImmutableDictionary<string, string> ConfigurationDimensions { get; }

        public DataTemplate? Template => ConfigurationDimensions.IsEmpty ? Parent?.Metadata.Editor?.UnconfiguredDataTemplate : Parent?.Metadata.Editor?.ConfiguredDataTemplate;

        public Setting? Parent { get; internal set; }

        public ImmutableArray<string> EnumValues
        {
            get => _enumValues;
            set
            {
                _enumValues = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets and sets the current value of the property.
        /// </summary>
        public object Value
        {
            get => _value;
            set
            {
                if (!Equals(value, Value))
                {
                    _value = value;
                    OnPropertyChanged();
                }
            }
        }

        public SettingValue Clone() => new SettingValue(ConfigurationDimensions, _value);

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}