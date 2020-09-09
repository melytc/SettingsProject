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
    internal abstract class Setting : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool _isSearchVisible = true;
        private bool _isConditionalVisible = true;
        private List<(Setting target, object visibleWhenValue)>? _dependentTargets;
        private ImmutableArray<ISettingValue> _values;

        protected SettingMetadata Metadata { get; }

        public string Name => Metadata.Name;
        public string Page => Metadata.Page;
        public string Category => Metadata.Category;
        public string? Description => Metadata.Description;
        public int Priority => Metadata.Priority;
        public SettingIdentity Identity => Metadata.Identity;
        public bool SupportsPerConfigurationValues => Metadata.SupportsPerConfigurationValues;

        public virtual bool HasDescription => !string.IsNullOrWhiteSpace(Metadata.Description);

        public bool HasPerConfigurationValues => Values.Any(value => value.Configuration != null);

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

        protected Setting(SettingMetadata metadata)
        {
            Metadata = metadata;
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

    internal sealed class StringSetting : Setting
    {
        public StringSetting(string name, string? description, string page, string category, int priority, UnconfiguredStringSettingValue value, bool supportsPerConfigurationValues = false)
            : this(new SettingMetadata(name, page, category, description, priority, supportsPerConfigurationValues))
        {
            Values = ImmutableArray.Create<ISettingValue>(value);
        }

        public StringSetting(string name, string? description, string page, string category, int priority, params ConfiguredStringSettingValue[] values)
            : this(new SettingMetadata(name, page, category, description, priority, supportsPerConfigurationValues: true))
        {
            Values = values.ToImmutableArray<ISettingValue>();
        }

        private StringSetting(SettingMetadata metadata)
            : base(metadata)
        {
        }

        public override Setting Clone() => new StringSetting(Metadata) { Values = Values.Select(value => value.Clone()).ToImmutableArray() };
    }

    internal sealed class UnconfiguredStringSettingValue : SettingValue<string>
    {
        private static readonly DataTemplate _template = (DataTemplate)Application.Current.FindResource("UnconfiguredStringSettingValueTemplate");

        public UnconfiguredStringSettingValue(string value, IEqualityComparer<string>? comparer = null)
            : base(value, comparer)
        {
        }

        public override DataTemplate Template => _template;
        public override string? Configuration => null;
        public override ISettingValue Clone() => new UnconfiguredStringSettingValue(Value, Comparer);
    }

    internal sealed class ConfiguredStringSettingValue : SettingValue<string>
    {
        private static readonly DataTemplate _template = (DataTemplate)Application.Current.FindResource("ConfiguredStringSettingValueTemplate");

        public ConfiguredStringSettingValue(string configuration, string value, IEqualityComparer<string>? comparer = null)
            : base(value, comparer)
        {
            Configuration = configuration;
        }

        public override DataTemplate Template => _template;
        public override string Configuration { get; }
        public override ISettingValue Clone() => new ConfiguredStringSettingValue(Configuration, Value, Comparer);
    }

    internal sealed class MultiLineStringSetting : Setting
    {
        public MultiLineStringSetting(string name, string? description, string page, string category, int priority, UnconfiguredMultilineStringSettingValue value, bool supportsPerConfigurationValues = false)
            : this(new SettingMetadata(name, page, category, description, priority, supportsPerConfigurationValues))
        {
            Values = ImmutableArray.Create<ISettingValue>(value);
        }

        public MultiLineStringSetting(string name, string? description, string page, string category, int priority, params ConfiguredMultilineStringSettingValue[] values)
            : this(new SettingMetadata(name, page, category, description, priority, supportsPerConfigurationValues: true))
        {
            Values = values.ToImmutableArray<ISettingValue>();
        }

        private MultiLineStringSetting(SettingMetadata metadata)
            : base(metadata)
        {
        }

        public override Setting Clone() => new MultiLineStringSetting(Metadata) { Values = Values.Select(value => value.Clone()).ToImmutableArray() };
    }

    internal sealed class UnconfiguredMultilineStringSettingValue : SettingValue<string>
    {
        private static readonly DataTemplate _template = (DataTemplate)Application.Current.FindResource("UnconfiguredMultilineStringSettingValueTemplate");

        public UnconfiguredMultilineStringSettingValue(string value, IEqualityComparer<string>? comparer = null)
            : base(value, comparer)
        {
        }

        public override DataTemplate Template => _template;
        public override string? Configuration => null;
        public override ISettingValue Clone() => new UnconfiguredMultilineStringSettingValue(Value, Comparer);
    }

    internal sealed class ConfiguredMultilineStringSettingValue : SettingValue<string>
    {
        private static readonly DataTemplate _template = (DataTemplate)Application.Current.FindResource("ConfiguredMultilineStringSettingValueTemplate");

        public ConfiguredMultilineStringSettingValue(string configuration, string value, IEqualityComparer<string>? comparer = null)
            : base(value, comparer)
        {
            Configuration = configuration;
        }

        public override DataTemplate Template => _template;
        public override string Configuration { get; }
        public override ISettingValue Clone() => new ConfiguredMultilineStringSettingValue(Configuration, Value, Comparer);
    }

    internal sealed class BoolSetting : Setting
    {
        public BoolSetting(string name, string? description, string page, string category, int priority, UnconfiguredBoolSettingValue value, bool supportsPerConfigurationValues = false)
            : this(new SettingMetadata(name, page, category, description, priority, supportsPerConfigurationValues))
        {
            Values = ImmutableArray.Create<ISettingValue>(value);
        }

        public BoolSetting(string name, string? description, string page, string category, int priority, params ConfiguredBoolSettingValue[] values)
            : this(new SettingMetadata(name, page, category, description, priority, supportsPerConfigurationValues: true))
        {
            Values = values.ToImmutableArray<ISettingValue>();
        }

        private BoolSetting(SettingMetadata metadata)
            : base(metadata)
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
        
        public override Setting Clone() => new BoolSetting(Metadata) { Values = Values.Select(value => value.Clone()).ToImmutableArray() };
    }

    internal sealed class UnconfiguredBoolSettingValue : SettingValue<bool>
    {
        private static readonly DataTemplate _template = (DataTemplate)Application.Current.FindResource("UnconfiguredBoolSettingValueTemplate");

        public BoolSetting? Parent { get; internal set; }

        public UnconfiguredBoolSettingValue(bool value, IEqualityComparer<bool>? comparer = null)
            : base(value, comparer)
        {
        }

        public override DataTemplate Template => _template;
        public override string? Configuration => null;
        public override ISettingValue Clone() => new UnconfiguredBoolSettingValue(Value, Comparer);
    }

    internal sealed class ConfiguredBoolSettingValue : SettingValue<bool>
    {
        private static readonly DataTemplate _template = (DataTemplate)Application.Current.FindResource("ConfiguredBoolSettingValueTemplate");

        public BoolSetting? Parent { get; internal set; }

        public ConfiguredBoolSettingValue(string configuration, bool value, IEqualityComparer<bool>? comparer = null)
            : base(value, comparer)
        {
            Configuration = configuration;
        }

        public override DataTemplate Template => _template;
        public override string Configuration { get; }
        public override ISettingValue Clone() => new ConfiguredBoolSettingValue(Configuration, Value, Comparer);
    }

    internal sealed class EnumSetting : Setting
    {
        public IReadOnlyList<string> EnumValues { get; }

        // Note: We might want to use IEnumValue here.
        public EnumSetting(string name, string? description, string page, string category, int priority, IReadOnlyList<string> enumValues, UnconfiguredEnumSettingValue value, bool supportsPerConfigurationValues = false)
            : this(new SettingMetadata(name, page, category, description, priority, supportsPerConfigurationValues), enumValues)
        {
            Values = ImmutableArray.Create<ISettingValue>(value);
        }

        public EnumSetting(string name, string? description, string page, string category, int priority, IReadOnlyList<string> enumValues, IReadOnlyList<ConfiguredEnumSettingValue> values)
            : this(new SettingMetadata(name, page, category, description, priority, supportsPerConfigurationValues: true), enumValues)
        {
            Values = values.ToImmutableArray<ISettingValue>();
        }

        private EnumSetting(SettingMetadata metadata, IReadOnlyList<string> enumValues)
            : base(metadata)
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

        public override Setting Clone() => new EnumSetting(Metadata, EnumValues) { Values = Values.Select(value => value.Clone()).ToImmutableArray() };
    }

    internal sealed class UnconfiguredEnumSettingValue : SettingValue<string>
    {
        private static readonly DataTemplate _template = (DataTemplate)Application.Current.FindResource("UnconfiguredEnumSettingValueTemplate");

        public EnumSetting? Parent { get; internal set; }

        public UnconfiguredEnumSettingValue(string value, IEqualityComparer<string>? comparer = null)
            : base(value, comparer)
        {
        }

        public override DataTemplate Template => _template;
        public override string? Configuration => null;
        public override ISettingValue Clone() => new UnconfiguredEnumSettingValue(Value, Comparer);
    }

    internal sealed class ConfiguredEnumSettingValue : SettingValue<string>
    {
        private static readonly DataTemplate _template = (DataTemplate)Application.Current.FindResource("ConfiguredEnumSettingValueTemplate");

        public EnumSetting? Parent { get; internal set; }

        public ConfiguredEnumSettingValue(string configuration, string value, IEqualityComparer<string>? comparer = null)
            : base(value, comparer)
        {
            Configuration = configuration;
        }

        public override DataTemplate Template => _template;
        public override string Configuration { get; }
        public override ISettingValue Clone() => new ConfiguredEnumSettingValue(Configuration, Value, Comparer);
    }

    internal sealed class LinkAction : Setting
    {
        public LinkAction(string name, string? description, string page, string category, int priority)
            : base(new SettingMetadata(name, page, category, description, priority, supportsPerConfigurationValues: false))
        {
            Values = ImmutableArray<ISettingValue>.Empty;
        }

        private LinkAction(SettingMetadata metadata)
            : base(metadata)
        {
        }

        public string HeadingText => HasDescription ? Name : "";
        
        public string LinkText => Description ?? Name;

        public override Setting Clone() => new LinkAction(Metadata);
    }
}
