using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable

namespace SettingsProject
{
    internal enum SettingModificationState
    {
        Default,
        Modified,
        ModifiedUnsaved
    }

    abstract class Setting
    {
        public string Name { get; }

        public string? Description { get; }

        /// <summary>
        /// Relative priority of the setting, to use when ordering items in the UI.
        /// </summary>
        public int Priority { get; }

        public abstract SettingModificationState ModificationState { get; protected set; }

        protected Setting(string name, string? description, int priority)
        {
            Name = name;
            Description = description;
            Priority = priority;
        }
    }

    abstract class Setting<T> : Setting, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        
        private readonly T _initialValue;
        private readonly T _defaultValue;
        private readonly IEqualityComparer<T> _comparer;
        private T _value;

        //public string? Description { get; }

        public override SettingModificationState ModificationState { get; protected set; } = SettingModificationState.Default;

        /// <summary>
        /// Gets and sets the current value of the property.
        /// </summary>
        public T Value
        {
            get => _value;
            set
            {
                if (!_comparer.Equals(value, Value))
                {
                    _value = value;
                    OnPropertyChanged(nameof(Value));

                    // ModificationState can only change when Value changes

                    var state = SettingModificationState.Default;
                    if (!_comparer.Equals(value, _initialValue))
                    {
                        state = SettingModificationState.ModifiedUnsaved;
                    }
                    else if (!_comparer.Equals(value, _defaultValue))
                    {
                        state = SettingModificationState.Modified;
                    }

                    if (state != ModificationState)
                    {
                        ModificationState = state;
                        OnPropertyChanged(nameof(ModificationState));
                    }
                }
            }
        }

#pragma warning disable CS8618 // _value is not initialized.
        protected Setting(string name, T initialValue, T defaultValue, string? description, int priority, IEqualityComparer<T> comparer)
#pragma warning restore CS8618 // _value is not initialized.
            : base(name, description, priority)
        {
            _initialValue = initialValue;
            _defaultValue = defaultValue;
            _comparer = comparer;
            Value = initialValue;
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    class StringSetting : Setting<string>
    {
        public StringSetting(string name, string initialValue, string? defaultValue, string description, int priority, IEqualityComparer<string>? comparer = null)
            : base(name, initialValue, defaultValue ?? "", description, priority, comparer ?? StringComparer.Ordinal)
        {
        }
    }

    class MultiLineStringSetting : Setting<string>
    {
        public MultiLineStringSetting(string name, string initialValue, string? defaultValue, string description, int priority, IEqualityComparer<string>? comparer = null)
            : base(name, initialValue, defaultValue ?? "", description, priority, comparer ?? StringComparer.Ordinal)
        {
        }
    }

    class BoolSetting : Setting<bool>
    {
        public BoolSetting(string name, bool initialValue, bool? defaultValue, string description, int priority)
            : base(name, initialValue, defaultValue ?? false, description, priority, EqualityComparer<bool>.Default)
        {
        }
    }

    class EnumSetting : Setting<string>
    {
        public List<string> EnumValues { get; }

        // Note: We might want to use IEnumValue here.
        public EnumSetting(string name, string initialValue, string? defaultValue, List<string> enumValues, string description, int priority, IEqualityComparer<string>? comparer = null)
            : base(name, initialValue, defaultValue ?? "", description, priority, comparer ?? StringComparer.Ordinal)
        {
            EnumValues = enumValues;
        }
    }
}
