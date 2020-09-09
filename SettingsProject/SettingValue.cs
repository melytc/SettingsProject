using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

#nullable enable

namespace SettingsProject
{
    internal interface ISettingValue : INotifyPropertyChanged
    {
        // null if this value applies to all configurations
        public string? Configuration { get; }

        public DataTemplate Template { get; }

        public SettingModificationState ModificationState { get; }

        public object Value { get; }
        
        ISettingValue Clone();
    }

    internal abstract class SettingValue<T> : ISettingValue where T : notnull
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected IEqualityComparer<T> Comparer { get; }

        private T _value;

        public T DefaultValue { get; }
        public T InitialValue { get; }

        protected SettingValue(T initialValue, T defaultValue, IEqualityComparer<T>? comparer = null)
        {
            Comparer = comparer ?? EqualityComparer<T>.Default;
            InitialValue = initialValue;
            DefaultValue = defaultValue;
            _value = initialValue;
            UpdateModificationState();
        }

        public SettingModificationState ModificationState { get; private set; } = SettingModificationState.Default;

        object ISettingValue.Value => Value;

        public abstract string? Configuration { get; }
        public abstract DataTemplate Template { get; }

        /// <summary>
        /// Gets and sets the current value of the property.
        /// </summary>
        public T Value
        {
            get => _value;
            set
            {
                if (!Comparer.Equals(value, Value))
                {
                    _value = value;
                    OnPropertyChanged(nameof(Value));
                    UpdateModificationState();
                }
            }
        }

        private void UpdateModificationState()
        {
            // ModificationState can only change when Value changes

            var state = SettingModificationState.Default;
            if (!Comparer.Equals(_value, InitialValue))
            {
                state = SettingModificationState.ModifiedUnsaved;
            }
            else if (!Comparer.Equals(_value, DefaultValue))
            {
                state = SettingModificationState.Modified;
            }

            if (state != ModificationState)
            {
                ModificationState = state;
                OnPropertyChanged(nameof(ModificationState));
            }
        }

        public abstract ISettingValue Clone();

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}