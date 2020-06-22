using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

#nullable enable

namespace SettingsProject
{
    internal enum SettingModificationState
    {
        Default,
        Modified,
        ModifiedUnsaved
    }

    internal abstract class SettingValue : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        // null if this value applies to all configurations
        public abstract string? Configuration { get; }

        public abstract DataTemplate Template { get; }

        public abstract SettingModificationState ModificationState { get; }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    internal abstract class SettingValue<T> : SettingValue
    {
        private readonly IEqualityComparer<T> _comparer;

        private SettingModificationState _modificationState = SettingModificationState.Default;
        private T _value;

        public T DefaultValue { get; }
        public T InitialValue { get; }

        protected SettingValue(T initialValue, T defaultValue, IEqualityComparer<T>? comparer = null)
        {
            _comparer = comparer ?? EqualityComparer<T>.Default;
            InitialValue = initialValue;
            DefaultValue = defaultValue;
            _value = initialValue;
            UpdateModificationState();
        }

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
                    UpdateModificationState();
                }
            }
        }

        private void UpdateModificationState()
        {
            // ModificationState can only change when Value changes

            var state = SettingModificationState.Default;
            if (!_comparer.Equals(_value, InitialValue))
            {
                state = SettingModificationState.ModifiedUnsaved;
            }
            else if (!_comparer.Equals(_value, DefaultValue))
            {
                state = SettingModificationState.Modified;
            }

            if (state != ModificationState)
            {
                _modificationState = state;
                OnPropertyChanged(nameof(ModificationState));
            }
        }
    }

    // For drop-down menu on setting itself
    internal class SettingCommand
    {
        public string Caption { get; }

        // null if this command is not checkable
        public bool? IsChecked { get; }

        public SettingCommand(string caption)
        {
            Caption = caption;
        }

        public void Invoke(Setting setting)
        {
        }
    }

    internal abstract class Setting : INotifyPropertyChanged
    {
        public static ImmutableArray<SettingCommand> ToggleConfigurationCommands { get; } = ImmutableArray.Create(new SettingCommand("Use single value across configurations"), new SettingCommand("Specify value per configuration"));

        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly string? _description;
        private bool _isVisible = true;

        public string Name { get; }

        public string Page { get; }

        public string Category { get; }

        public virtual bool HasDescription => !string.IsNullOrWhiteSpace(_description);

        public string Description => _description ?? "";

        /// <summary>
        /// Relative priority of the setting, to use when ordering items in the UI.
        /// </summary>
        public int Priority { get; }

        public ImmutableArray<SettingCommand> Commands { get; }

        public ImmutableArray<SettingValue> Values { get; }

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible == value)
                    return;
                _isVisible = value;
                OnPropertyChanged();
            }
        }

        public bool HasCommands => !Commands.IsEmpty;

        protected Setting(string name, string? description, string page, string category, int priority, SettingValue value, ImmutableArray<SettingCommand> commands)
            : this(name, description, page, category, priority, ImmutableArray.Create(value), commands)
        {
        }

        protected Setting(string name, string? description, string page, string category, int priority, ImmutableArray<SettingValue> values, ImmutableArray<SettingCommand> commands)
        {
            Name = name;
            _description = description;
            Page = page;
            Category = category;
            Priority = priority;
            Values = values;
            Commands = commands;
        }

        public virtual bool MatchesSearchText(string searchString)
        {
            return Name.IndexOf(searchString, StringComparison.CurrentCultureIgnoreCase) != -1 
                || (Description != null && Description.IndexOf(searchString, StringComparison.CurrentCultureIgnoreCase) != -1);
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    internal class StringSetting : Setting
    {
        public StringSetting(string name, string? description, string page, string category, int priority, UnconfiguredStringSettingValue value, bool supportsPerConfigurationValues = false)
            : base(name, description, page, category, priority, value, supportsPerConfigurationValues ? ToggleConfigurationCommands : ImmutableArray<SettingCommand>.Empty)
        {
        }

        public StringSetting(string name, string? description, string page, string category, int priority, params ConfiguredStringSettingValue[] values)
            : base(name, description, page, category, priority, values.ToImmutableArray<SettingValue>(), ToggleConfigurationCommands)
        {
        }
    }

    internal sealed class UnconfiguredStringSettingValue : SettingValue<string>
    {
        private static readonly DataTemplate _template = (DataTemplate)Application.Current.FindResource("UnconfiguredStringSettingValueTemplate");

        public UnconfiguredStringSettingValue(string initialValue, string? defaultValue, IEqualityComparer<string>? comparer = null)
            : base(initialValue, defaultValue ?? "", comparer)
        {
        }

        public override DataTemplate Template => _template;
        public override string? Configuration => null;
    }

    internal sealed class ConfiguredStringSettingValue : SettingValue<string>
    {
        private static readonly DataTemplate _template = (DataTemplate)Application.Current.FindResource("ConfiguredStringSettingValueTemplate");

        public ConfiguredStringSettingValue(string configuration, string initialValue, string? defaultValue, IEqualityComparer<string>? comparer = null)
            : base(initialValue, defaultValue ?? "", comparer)
        {
            Configuration = configuration;
        }

        public override DataTemplate Template => _template;
        public override string? Configuration { get; }
    }

    internal class MultiLineStringSetting : Setting
    {
        public MultiLineStringSetting(string name, string initialValue, string? defaultValue, string? description, int priority, string page, string category, IEqualityComparer<string>? comparer = null, bool supportsPerConfigurationValues = false)
            : base(name, description, page, category, priority, new UnconfiguredMultilineStringSettingValue(initialValue, defaultValue ?? "", comparer ?? StringComparer.Ordinal), supportsPerConfigurationValues ? ToggleConfigurationCommands : ImmutableArray<SettingCommand>.Empty)
        {
        }
    }

    internal sealed class UnconfiguredMultilineStringSettingValue : SettingValue<string>
    {
        private static readonly DataTemplate _template = (DataTemplate)Application.Current.FindResource("UnconfiguredMultilineStringSettingValueTemplate");

        public UnconfiguredMultilineStringSettingValue(string initialValue, string defaultValue, IEqualityComparer<string>? comparer = null)
            : base(initialValue, defaultValue, comparer)
        {
        }

        public override DataTemplate Template => _template;
        public override string? Configuration => null;
    }

    internal sealed class ConfiguredMultilineStringSettingValue : SettingValue<string>
    {
        private static readonly DataTemplate _template = (DataTemplate)Application.Current.FindResource("ConfiguredMultilineStringSettingValueTemplate");

        public ConfiguredMultilineStringSettingValue(string configuration, string initialValue, string defaultValue, IEqualityComparer<string>? comparer = null)
            : base(initialValue, defaultValue, comparer)
        {
            Configuration = configuration;
        }

        public override DataTemplate Template => _template;
        public override string? Configuration { get; }
    }

    internal class BoolSetting : Setting
    {
        public BoolSetting(string name, string? description, string page, string category, int priority, UnconfiguredBoolSettingValue value, bool supportsPerConfigurationValues = false)
            : base(name, description, page, category, priority, value, supportsPerConfigurationValues ? ToggleConfigurationCommands : ImmutableArray<SettingCommand>.Empty)
        {
            value.Parent = this;
        }

        public BoolSetting(string name, string? description, string page, string category, int priority, params ConfiguredBoolSettingValue[] values)
            : base(name, description, page, category, priority, values.ToImmutableArray<SettingValue>(), ToggleConfigurationCommands)
        {
            foreach (var value in values)
            {
                value.Parent = this;
            }
        }

        public override bool HasDescription => Values.All(value => value.Configuration != null);
    }

    internal sealed class UnconfiguredBoolSettingValue : SettingValue<bool>
    {
        private static readonly DataTemplate _template = (DataTemplate)Application.Current.FindResource("UnconfiguredBoolSettingValueTemplate");

        public BoolSetting? Parent { get; internal set; }

        // TODO allow a null default value?
        public UnconfiguredBoolSettingValue(bool initialValue, bool? defaultValue, IEqualityComparer<bool>? comparer = null)
            : base(initialValue, defaultValue ?? false, comparer)
        {
        }

        public override DataTemplate Template => _template;
        public override string? Configuration => null;
    }

    internal sealed class ConfiguredBoolSettingValue : SettingValue<bool>
    {
        private static readonly DataTemplate _template = (DataTemplate)Application.Current.FindResource("ConfiguredBoolSettingValueTemplate");

        public BoolSetting? Parent { get; internal set; }

        // TODO allow a null default value?
        public ConfiguredBoolSettingValue(string configuration, bool initialValue, bool? defaultValue, IEqualityComparer<bool>? comparer = null)
            : base(initialValue, defaultValue ?? false, comparer)
        {
            Configuration = configuration;
        }

        public override DataTemplate Template => _template;
        public override string? Configuration { get; }
    }

    internal class EnumSetting : Setting
    {
        public IReadOnlyList<string> EnumValues { get; }

        // Note: We might want to use IEnumValue here.
        public EnumSetting(string name, string? description, string page, string category, int priority, IReadOnlyList<string> enumValues, UnconfiguredEnumSettingValue value, bool supportsPerConfigurationValues = false)
            : base(name, description, page, category, priority, value, supportsPerConfigurationValues ? ToggleConfigurationCommands : ImmutableArray<SettingCommand>.Empty)
        {
            EnumValues = enumValues;
            value.Parent = this;
        }

        public EnumSetting(string name, string? description, string page, string category, int priority, IReadOnlyList<string> enumValues, IReadOnlyList<ConfiguredEnumSettingValue> values)
            : base(name, description, page, category, priority, values.ToImmutableArray<SettingValue>(), ToggleConfigurationCommands)
        {
            EnumValues = enumValues;
            
            foreach (var value in values)
            {
                value.Parent = this;
            }
        }

        public override bool MatchesSearchText(string searchString)
        {
            if (base.MatchesSearchText(searchString))
            {
                return true;
            }

            foreach (var enumValue in EnumValues)
            {
                if (enumValue.IndexOf(searchString, StringComparison.CurrentCultureIgnoreCase) != -1)
                    return true;
            }

            return false;
        }
    }

    internal sealed class UnconfiguredEnumSettingValue : SettingValue<string>
    {
        private static readonly DataTemplate _template = (DataTemplate)Application.Current.FindResource("UnconfiguredEnumSettingValueTemplate");

        public EnumSetting? Parent { get; internal set; }

        public UnconfiguredEnumSettingValue(string initialValue, string? defaultValue, IEqualityComparer<string>? comparer = null)
            : base(initialValue, defaultValue ?? "", comparer)
        {
        }

        public override DataTemplate Template => _template;
        public override string? Configuration => null;
    }

    internal sealed class ConfiguredEnumSettingValue : SettingValue<string>
    {
        private static readonly DataTemplate _template = (DataTemplate)Application.Current.FindResource("ConfiguredEnumSettingValueTemplate");

        public EnumSetting? Parent { get; internal set; }

        public ConfiguredEnumSettingValue(string configuration, string initialValue, string? defaultValue, IEqualityComparer<string>? comparer = null)
            : base(initialValue, defaultValue ?? "", comparer)
        {
            Configuration = configuration;
        }

        public override DataTemplate Template => _template;
        public override string? Configuration { get; }
    }

    internal class LinkAction : Setting
    {
        public LinkAction(string name, string? description, string page, string category, int priority)
            : base(name, description, page, category, priority, ImmutableArray<SettingValue>.Empty, ImmutableArray<SettingCommand>.Empty)
        {
        }

        public string HeadingText => HasDescription ? Name : "";
        
        public string LinkText => HasDescription ? Description : Name;
    }
}
