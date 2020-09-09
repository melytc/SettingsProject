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

    internal abstract class Setting : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly string? _description;
        private bool _isSearchVisible = true;
        private bool _isConditionalVisible = true;
        private List<(Setting target, object visibleWhenValue)>? _dependentTargets;
        private ImmutableArray<ISettingValue> _values;

        public string Name { get; }

        public string Page { get; }

        public string Category { get; }

        public virtual bool HasDescription => !string.IsNullOrWhiteSpace(_description);

        public bool HasPerConfigurationValues => Values.Any(value => value.Configuration != null);

        public bool SupportsPerConfigurationValues { get; }

        public string Description => _description ?? "";

        /// <summary>
        /// Relative priority of the setting, to use when ordering items in the UI.
        /// </summary>
        public int Priority { get; }

        public ImmutableArray<ISettingValue> Values
        {
            get => _values;
            set
            {
                _values = value;
                
                foreach (var settingValue in value)
                {
                    OnValueAdded(settingValue);
                }

                OnPropertyChanged();
                OnPropertyChanged(nameof(HasPerConfigurationValues));
            }
        }

        protected virtual void OnValueAdded(ISettingValue value)
        {
            value.PropertyChanged += OnPropertyChanged;

            void OnPropertyChanged(object _, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(SettingValue<bool>.Value))
                {
                   UpdateDependentVisibilities();
                }
            }
        }

        public bool IsVisible => _isSearchVisible && _isConditionalVisible;

        public SettingIdentity Identity => new SettingIdentity(Page, Category, Name);

        protected Setting(string name, string? description, string page, string category, int priority, bool supportsPerConfigurationValues)
        {
            Name = name;
            _description = description;
            Page = page;
            Category = category;
            Priority = priority;
            SupportsPerConfigurationValues = supportsPerConfigurationValues;
        }

        private void UpdateDependentVisibilities()
        {
            // TODO model this as a graph with edges so that multiple upstream properties may influence a single downstream one

            if (_dependentTargets == null)
            {
                return;
            }

            foreach (var (target, visibleWhenValue) in _dependentTargets)
            {
                var wasVisible = target.IsVisible;

                bool isConditionallyVisible = false;

                // Target is visible if any upstream value matches
                foreach (var value in Values)
                {
                    if (Equals(visibleWhenValue, value.Value))
                    {
                        isConditionallyVisible = true;
                        break;
                    }
                }

                target._isConditionalVisible = isConditionallyVisible;

                if (wasVisible != target.IsVisible)
                {
                    target.OnPropertyChanged(nameof(IsVisible));
                }
            }
        }

        protected virtual bool MatchesSearchText(string searchString) => false;

        public void AddDependentTarget(Setting target, object visibleWhenValue)
        {
            _dependentTargets ??= new List<(Setting target, object visibleWhenValue)>();

            _dependentTargets.Add((target, visibleWhenValue));

            UpdateDependentVisibilities();
        }

        public void UpdateSearchState(string searchString)
        {
            var wasVisible = IsVisible;

            _isSearchVisible = Name.IndexOf(searchString, StringComparison.CurrentCultureIgnoreCase) != -1
                || (Description != null && Description.IndexOf(searchString, StringComparison.CurrentCultureIgnoreCase) != -1)
                || MatchesSearchText(searchString);

            if (wasVisible != IsVisible)
            {
                OnPropertyChanged(nameof(IsVisible));
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public abstract Setting Clone();
    }

    internal class StringSetting : Setting
    {
        public StringSetting(string name, string? description, string page, string category, int priority, UnconfiguredStringSettingValue value, bool supportsPerConfigurationValues = false)
            : this(name, description, page, category, priority, supportsPerConfigurationValues)
        {
            Values = ImmutableArray.Create<ISettingValue>(value);
        }

        public StringSetting(string name, string? description, string page, string category, int priority, params ConfiguredStringSettingValue[] values)
            : this(name, description, page, category, priority, supportsPerConfigurationValues: true)
        {
            Values = values.ToImmutableArray<ISettingValue>();
        }

        private StringSetting(string name, string? description, string page, string category, int priority, bool supportsPerConfigurationValues)
            : base(name, description, page, category, priority, supportsPerConfigurationValues)
        {
        }

        public override Setting Clone() => new StringSetting(Name, Description, Page, Category, Priority) { Values = Values.Select(value => value.Clone()).ToImmutableArray() };
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
        public override ISettingValue Clone() => new UnconfiguredStringSettingValue(InitialValue, DefaultValue, Comparer);
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
        public override string Configuration { get; }
        public override ISettingValue Clone() => new ConfiguredStringSettingValue(Configuration, InitialValue, DefaultValue, Comparer);
    }

    internal class MultiLineStringSetting : Setting
    {
        public MultiLineStringSetting(string name, string? description, string page, string category, int priority, UnconfiguredMultilineStringSettingValue value, bool supportsPerConfigurationValues = false)
            : this(name, description, page, category, priority, supportsPerConfigurationValues)
        {
            Values = ImmutableArray.Create<ISettingValue>(value);
        }

        public MultiLineStringSetting(string name, string? description, string page, string category, int priority, params ConfiguredMultilineStringSettingValue[] values)
            : this(name, description, page, category, priority, supportsPerConfigurationValues: true)
        {
            Values = values.ToImmutableArray<ISettingValue>();
        }

        private MultiLineStringSetting(string name, string? description, string page, string category, int priority, bool supportsPerConfigurationValues)
            : base(name, description, page, category, priority, supportsPerConfigurationValues)
        {
        }

        public override Setting Clone() => new MultiLineStringSetting(Name, Description, Page, Category, Priority, SupportsPerConfigurationValues) { Values = Values.Select(value => value.Clone()).ToImmutableArray() };
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
        public override ISettingValue Clone() => new UnconfiguredMultilineStringSettingValue(InitialValue, DefaultValue, Comparer);
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
        public override string Configuration { get; }
        public override ISettingValue Clone() => new ConfiguredMultilineStringSettingValue(Configuration, InitialValue, DefaultValue, Comparer);
    }

    internal class BoolSetting : Setting
    {
        public BoolSetting(string name, string? description, string page, string category, int priority, UnconfiguredBoolSettingValue value, bool supportsPerConfigurationValues = false)
            : this(name, description, page, category, priority, supportsPerConfigurationValues)
        {
            Values = ImmutableArray.Create<ISettingValue>(value);
        }

        public BoolSetting(string name, string? description, string page, string category, int priority, params ConfiguredBoolSettingValue[] values)
            : this(name, description, page, category, priority, supportsPerConfigurationValues: true)
        {
            Values = values.ToImmutableArray<ISettingValue>();
        }

        private BoolSetting(string name, string? description, string page, string category, int priority, bool supportsPerConfigurationValues)
            : base(name, description, page, category, priority, supportsPerConfigurationValues)
        {
        }

        protected override void OnValueAdded(ISettingValue value)
        {
            switch (value)
            {
                case UnconfiguredBoolSettingValue unconfigured:
                {
                    unconfigured.Parent = this;
                    break;
                }
                case ConfiguredBoolSettingValue configured:
                {
                    configured.Parent = this;
                    break;
                }
            }
        }

        public override bool HasDescription => Values.All(value => value.Configuration != null);
        
        public override Setting Clone() => new BoolSetting(Name, Description, Page, Category, Priority, SupportsPerConfigurationValues) { Values = Values.Select(value => value.Clone()).ToImmutableArray() };
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
        public override ISettingValue Clone() => new UnconfiguredBoolSettingValue(InitialValue, DefaultValue, Comparer);
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
        public override string Configuration { get; }
        public override ISettingValue Clone() => new ConfiguredBoolSettingValue(Configuration, InitialValue, DefaultValue, Comparer);
    }

    internal class EnumSetting : Setting
    {
        public IReadOnlyList<string> EnumValues { get; }

        // Note: We might want to use IEnumValue here.
        public EnumSetting(string name, string? description, string page, string category, int priority, IReadOnlyList<string> enumValues, UnconfiguredEnumSettingValue value, bool supportsPerConfigurationValues = false)
            : this(name, description, page, category, priority, enumValues, supportsPerConfigurationValues)
        {
            Values = ImmutableArray.Create<ISettingValue>(value);
        }

        public EnumSetting(string name, string? description, string page, string category, int priority, IReadOnlyList<string> enumValues, IReadOnlyList<ConfiguredEnumSettingValue> values)
            : this(name, description, page, category, priority, enumValues, supportsPerConfigurationValues: true)
        {
            Values = values.ToImmutableArray<ISettingValue>();
        }

        private EnumSetting(string name, string? description, string page, string category, int priority, IReadOnlyList<string> enumValues, bool supportsPerConfigurationValues)
            : base(name, description, page, category, priority, supportsPerConfigurationValues)
        {
            EnumValues = enumValues;
        }

        protected override void OnValueAdded(ISettingValue value)
        {
            switch (value)
            {
                case UnconfiguredEnumSettingValue unconfigured:
                {
                    unconfigured.Parent = this;
                    break;
                }
                case ConfiguredEnumSettingValue configured:
                {
                    configured.Parent = this;
                    break;
                }
            }
        }

        protected override bool MatchesSearchText(string searchString)
        {
            foreach (var enumValue in EnumValues)
            {
                if (enumValue.IndexOf(searchString, StringComparison.CurrentCultureIgnoreCase) != -1)
                    return true;
            }

            return false;
        }

        public override Setting Clone() => new EnumSetting(Name, Description, Page, Category, Priority, EnumValues, SupportsPerConfigurationValues) { Values = Values.Select(value => value.Clone()).ToImmutableArray() };
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
        public override ISettingValue Clone() => new UnconfiguredEnumSettingValue(InitialValue, DefaultValue, Comparer);
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
        public override string Configuration { get; }
        public override ISettingValue Clone() => new ConfiguredEnumSettingValue(Configuration, InitialValue, DefaultValue, Comparer);
    }

    internal class LinkAction : Setting
    {
        public LinkAction(string name, string? description, string page, string category, int priority)
            : base(name, description, page, category, priority, supportsPerConfigurationValues: false)
        {
            Values = ImmutableArray<ISettingValue>.Empty;
        }

        public string HeadingText => HasDescription ? Name : "";
        
        public string LinkText => HasDescription ? Description : Name;

        public override Setting Clone() => new LinkAction(Name, Description, Page, Category, Priority);
    }
}
