using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable enable

namespace SettingsProject
{
    internal class Setting : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool _isSearchVisible = true;
        private bool _isConditionalVisible = true;
        private List<(Setting target, object visibleWhenValue)>? _dependentTargets;
        private ImmutableArray<ISettingValue> _values;

        protected internal SettingMetadata Metadata { get; }

        public string Name => Metadata.Name;
        public string Page => Metadata.Page;
        public string Category => Metadata.Category;
        public string? Description => Metadata.Description;
        public int Priority => Metadata.Priority;
        public SettingIdentity Identity => Metadata.Identity;
        public ImmutableArray<string> EnumValues => Metadata.EnumValues;
        public bool SupportsPerConfigurationValues => Metadata.SupportsPerConfigurationValues;

        // TODO need to sort out something in the bool/checkbox template
        public bool HasDescription => !string.IsNullOrWhiteSpace(Metadata.Description);

        public bool HasPerConfigurationValues => Values.Any(value => value.Configuration != null);

        public ImmutableArray<ISettingValue> Values
        {
            get => _values;
            set
            {
                _values = value;
                
                foreach (var settingValue in value)
                {
                    settingValue.Parent = this;
                    settingValue.PropertyChanged += OnSettingValuePropertyChanged;

                    void OnSettingValuePropertyChanged(object _, PropertyChangedEventArgs e)
                    {
                        if (e.PropertyName == nameof(SettingValue.Value))
                        {
                            UpdateDependentVisibilities();
                        }
                    }
                }

                OnPropertyChanged();
                OnPropertyChanged(nameof(HasPerConfigurationValues));
            }
        }

        public bool IsVisible => _isSearchVisible && _isConditionalVisible;

        public Setting(string name, string? description, string page, string category, int priority, string editorType, UnconfiguredSettingValue value, ImmutableArray<string>? enumValues = null, bool supportsPerConfigurationValues = false)
            : this(new SettingMetadata(name, page, category, description, priority, editorType, supportsPerConfigurationValues, enumValues ?? ImmutableArray<string>.Empty))
        {
            Values = ImmutableArray.Create<ISettingValue>(value);
        }

        public Setting(string name, string? description, string page, string category, int priority, string editorType, ImmutableArray<string>? enumValues, ImmutableArray<ConfiguredSettingValue> values)
            : this(new SettingMetadata(name, page, category, description, priority, editorType, supportsPerConfigurationValues: true, enumValues ?? ImmutableArray<string>.Empty))
        {
            Values = values.CastArray<ISettingValue>();
        }

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

            bool MatchesSearchText(string searchString)
            {
                foreach (var enumValue in Metadata.EnumValues)
                {
                    if (enumValue.IndexOf(searchString, StringComparison.CurrentCultureIgnoreCase) != -1)
                        return true;
                }

                return false;
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual Setting Clone() => new Setting(Metadata) { Values = Values.Select(value => value.Clone()).ToImmutableArray() };
    }

    internal sealed class LinkAction : Setting
    {
        public LinkAction(string name, string? description, string page, string category, int priority)
            : base(new SettingMetadata(name, page, category, description, priority, null, supportsPerConfigurationValues: false, ImmutableArray<string>.Empty))
        {
            Values = ImmutableArray<ISettingValue>.Empty;
        }

        private LinkAction(SettingMetadata metadata)
            : base(metadata)
        {
            Values = ImmutableArray<ISettingValue>.Empty;
        }

        public string HeadingText => HasDescription ? Name : "";
        
        public string LinkText => Description ?? Name;

        public override Setting Clone() => new LinkAction(Metadata);
    }
}
