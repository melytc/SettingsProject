using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable

namespace SettingsProject
{
    abstract class Setting
    {
        public string Name { get; }

        /// <summary>
        /// Relative priority of the setting, to use when ordering items in the UI.
        /// </summary>
        public int Priority { get; }

        public abstract bool IsModified { get; protected set; }

        protected Setting(string name, int priority, bool isModified = false)
        {
            Name = name;
            Priority = priority;
        }
    }

    abstract class Setting<T> : Setting, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        
        private readonly T _defaultValue;
        private T _value;

        //public string? Description { get; }

        /// <summary>
        /// An optional comparer to use when evaluating value change events.
        /// </summary>
        /// <remarks>
        /// If an override does not exist, or returns <see langword="null"/>,
        /// the <see cref="EqualityComparer{T}.Default"/> is used.
        /// </remarks>
        public virtual IEqualityComparer<T>? Comparer => null;

        public override bool IsModified { get; protected set; }

        /// <summary>
        /// Gets and sets the current value of the property.
        /// </summary>
        public T Value
        {
            get => _value;
            set
            {
                var comparer = Comparer ?? EqualityComparer<T>.Default;

                if (!comparer.Equals(value, Value))
                {
                    // Only raise event when Value actually changes
                    _value = value;
                    OnPropertyChanged(nameof(Value));

                    // IsModified can only change when Value changes
                    var isModified = !comparer.Equals(value, _defaultValue);

                    if (isModified != IsModified)
                    {
                        // Only raise event when IsModified actually changes
                        IsModified = isModified;
                        OnPropertyChanged(nameof(IsModified));
                    }
                }

            }
        }

#pragma warning disable CS8618 // _value is not initialized.
        protected Setting(string name, T initialValue, T defaultValue, int priority)
#pragma warning restore CS8618 // _value is not initialized.
            : base(name, priority)
        {
            _defaultValue = defaultValue;
            Value = initialValue;
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    class StringSetting : Setting<string>
    {
        public override IEqualityComparer<string>? Comparer { get; }

        public StringSetting(string name, string initialValue, string defaultValue, int priority, IEqualityComparer<string>? comparer = null)
            : base(name, initialValue, defaultValue, priority)
        {
            Comparer = comparer;
        }
    }

    class MultiLineStringSetting : Setting<string>
    {
        public override IEqualityComparer<string>? Comparer { get; }

        public MultiLineStringSetting(string name, string initialValue, string defaultValue, int priority, IEqualityComparer<string>? comparer = null)
            : base(name, initialValue, defaultValue, priority)
        {
            Comparer = comparer;
        }
    }

    class BoolSetting : Setting<bool>
    {
        public BoolSetting(string name, bool initialValue, bool defaultValue, string description, int priority)
            : base(name, initialValue, defaultValue, priority)
        {
            Description = description;
        }

        public string Description { get; }
    }

    class EnumSetting : Setting<string>
    {
        public List<string> EnumValues { get; }

        // Note: We might want to use IEnumValue here.
        public EnumSetting(string name, string initialValue, string defaultValue, List<string> enumValues, int priority)
            : base(name, initialValue, defaultValue, priority)
        {
            EnumValues = enumValues;
        }
    }
}
