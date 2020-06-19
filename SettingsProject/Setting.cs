using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
        private readonly string? _description;

        public string Name { get; }

        public string Page { get; }

        public string Category { get; }

        public bool HasDescription => !string.IsNullOrWhiteSpace(_description);

        public string Description => _description ?? "";

        /// <summary>
        /// Relative priority of the setting, to use when ordering items in the UI.
        /// </summary>
        public int Priority { get; }

        public abstract SettingModificationState ModificationState { get; }

        protected Setting(string name, string? description, int priority, string page, string category)
        {
            Name = name;
            _description = description;
            Priority = priority;
            Page = page;
            Category = category;
        }

        public bool MatchesSearchText(string searchString)
        {
            return Name.IndexOf(searchString, StringComparison.CurrentCultureIgnoreCase) != -1 
                || (Description != null && Description.IndexOf(searchString, StringComparison.CurrentCultureIgnoreCase) != -1);
        }
    }

    abstract class Setting<T> : Setting, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        
        private readonly T _initialValue;
        private readonly T _defaultValue;
        private readonly IEqualityComparer<T> _comparer;
        private T _value;
        private SettingModificationState _modificationState = SettingModificationState.Default;

        public override SettingModificationState ModificationState => _modificationState;

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
                    OnValueChanged(_value);
                    UpdateModificationState();
                }
            }
        }

        private void UpdateModificationState()
        {
            // ModificationState can only change when Value changes

            var state = SettingModificationState.Default;
            if (!_comparer.Equals(_value, _initialValue))
            {
                state = SettingModificationState.ModifiedUnsaved;
            }
            else if (!_comparer.Equals(_value, _defaultValue))
            {
                state = SettingModificationState.Modified;
            }

            if (state != ModificationState)
            {
                _modificationState = state;
                OnPropertyChanged(nameof(ModificationState));
            }
        }

        protected virtual void OnValueChanged(T newValue)
        {
        }

#pragma warning disable CS8618 // _value is not initialized.
        protected Setting(string name, T initialValue, T defaultValue, string? description, int priority, string page, string category, IEqualityComparer<T> comparer)
#pragma warning restore CS8618 // _value is not initialized.
            : base(name, description, priority, page, category)
        {
            _initialValue = initialValue;
            _defaultValue = defaultValue;
            _comparer = comparer;
            _value = initialValue;
            UpdateModificationState();
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    class StringSetting : Setting<string>
    {
        public StringSetting(string name, string initialValue, string? defaultValue, string? description, int priority, string page, string category, IEqualityComparer<string>? comparer = null)
            : base(name, initialValue, defaultValue ?? "", description, priority, page, category, comparer ?? StringComparer.Ordinal)
        {
        }
    }

    class MultiLineStringSetting : Setting<string>
    {
        public MultiLineStringSetting(string name, string initialValue, string? defaultValue, string? description, int priority, string page, string category, IEqualityComparer<string>? comparer = null)
            : base(name, initialValue, defaultValue ?? "", description, priority, page, category, comparer ?? StringComparer.Ordinal)
        {
        }
    }

    class BoolSetting : Setting<bool>
    {
        public IReadOnlyList<Setting>? TrueSettings { get; }
        public IReadOnlyList<Setting>? FalseSettings { get; }

        public IReadOnlyList<Setting>? SelectedSettings { get; private set; }

        public BoolSetting(string name, bool initialValue, bool? defaultValue, string description, int priority, string page, string category, IReadOnlyList<Setting>? trueSettings = null, IReadOnlyList<Setting>? falseSettings = null)
            : base(name, initialValue, defaultValue ?? false, description, priority, page, category, EqualityComparer<bool>.Default)
        {
            TrueSettings = trueSettings;
            FalseSettings = falseSettings;
            OnValueChanged(Value);
        }

        protected override void OnValueChanged(bool newValue)
        {
            var newSettings = newValue ? TrueSettings : FalseSettings;

            if (!ReferenceEquals(SelectedSettings, newSettings))
            {
                SelectedSettings = newSettings;
                OnPropertyChanged(nameof(SelectedSettings));
            }
        }
    }

    class EnumSetting : Setting<string>
    {
        public IReadOnlyList<string> EnumValues { get; }

        // Note: We might want to use IEnumValue here.
        public EnumSetting(string name, string initialValue, string? defaultValue, IReadOnlyList<string> enumValues, string? description, int priority, string page, string category, IEqualityComparer<string>? comparer = null)
            : base(name, initialValue, defaultValue ?? "", description, priority, page, category, comparer ?? StringComparer.Ordinal)
        {
            EnumValues = enumValues;
        }
    }

    class LinkAction : Setting
    {
        public LinkAction(string name, int priority, string page, string category, string? description = null)
            : base(name, description, priority, page, category)
        {
        }

        public string HeadingText => HasDescription ? Name : "";
        
        public string LinkText => HasDescription ? Description : Name;

        public override SettingModificationState ModificationState => SettingModificationState.Default;
    }

    class RadioSetting : Setting<string>
    {
        private readonly IReadOnlyList<RadioOption> _options;

        public IReadOnlyList<Setting>? SelectedSettings { get; private set; }

        public string SelectedDescription { get; private set; } = "";

        public IEnumerable<string> EnumValues => _options.Select(option => option.Name);

        public RadioSetting(string name, string? description, int priority, string page, string category, IReadOnlyList<RadioOption> options, string initialValue, string defaultValue)
            : base(name, initialValue, defaultValue, description, priority, page, category, StringComparer.Ordinal)
        {
            _options = options;
            OnValueChanged(Value);
        }

        protected override void OnValueChanged(string newValue)
        {
            RadioOption? option = _options.FirstOrDefault(option => option.Name == newValue);

            if (!ReferenceEquals(SelectedSettings, option?.Settings))
            {
                SelectedSettings = option?.Settings;
                OnPropertyChanged(nameof(SelectedSettings));
            }

            string description = option?.Description ?? "";

            if (!Equals(SelectedDescription, description))
            {
                SelectedDescription = description;
                OnPropertyChanged(nameof(SelectedDescription));
            }
        }
    }

    class RadioOption
    {
        public string Name { get; }
        public string? Description { get; }
        public IReadOnlyList<Setting> Settings { get; }

        public RadioOption(string name, string? description, IReadOnlyList<Setting> settings)
        {
            Name = name;
            Description = description;
            Settings = settings;
        }
    }
}
