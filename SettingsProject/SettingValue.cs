using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

#nullable enable

namespace SettingsProject
{
    internal interface ISettingValue : INotifyPropertyChanged
    {
        // null if this value applies to all configurations
        string? Configuration { get; }

        DataTemplate? Template { get; }

        object Value { get; }

        Setting? Parent { get; set; }

        ISettingValue Clone();
    }

    internal abstract class SettingValue : ISettingValue
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private object _value;

        protected SettingValue(object value)
        {
            _value = value;
        }

        public abstract string? Configuration { get; }
        public abstract DataTemplate? Template { get; }

        public Setting? Parent { get; set; }

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

        public abstract ISettingValue Clone();

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    internal sealed class UnconfiguredSettingValue : SettingValue
    {
        public UnconfiguredSettingValue(object value)
            : base(value)
        {
        }

        public override DataTemplate? Template => Parent?.Metadata.Editor?.UnconfiguredDataTemplate;
        public override string? Configuration => null;
        public override ISettingValue Clone() => new UnconfiguredSettingValue(Value);
    }

    internal sealed class ConfiguredSettingValue : SettingValue
    {
        public ConfiguredSettingValue(string configuration, object value)
            : base(value)
        {
            Configuration = configuration;
        }

        public override DataTemplate? Template => Parent?.Metadata.Editor?.ConfiguredDataTemplate;
        public override string Configuration { get; }
        public override ISettingValue Clone() => new ConfiguredSettingValue(Configuration, Value);
    }
}