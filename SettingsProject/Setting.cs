using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;

#nullable enable

namespace SettingsProject
{
    internal sealed class Setting : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool _isSearchVisible = true;
        private bool _isConditionalVisible = true;
        private List<(Setting target, object visibleWhenValue)>? _dependentTargets;
        private ImmutableArray<SettingValue> _values;

        internal SettingMetadata Metadata { get; }
        internal SettingContext? Context { get; private set; }

        public string Name => Metadata.Name;
        public string Page => Metadata.Page;
        public string Category => Metadata.Category;
        public string? Description => Metadata.Description;
        public int Priority => Metadata.Priority;
        public SettingIdentity Identity => Metadata.Identity;
        public bool SupportsPerConfigurationValues => Metadata.SupportsPerConfigurationValues;

        public ImmutableArray<object> ConfigurationCommands { get; private set; }

        public ImmutableArray<SettingValue> Values
        {
            get => _values;
            set
            {
                // TODO validate incoming values
                // - set of dimensions across values must be identical
                // - number of values must match the dimension specifications

                _values = value;
                
                foreach (var settingValue in value)
                {
                    settingValue.Parent = this;
                    settingValue.PropertyChanged += OnSettingValuePropertyChanged;

                    void OnSettingValuePropertyChanged(object _, PropertyChangedEventArgs e)
                    {
                        if (e.PropertyName == nameof(SettingValue.Value) && _dependentTargets != null)
                        {
                            foreach (var (target, visibleWhenValue) in _dependentTargets)
                            {
                                UpdateDependentVisibility(target, visibleWhenValue);
                            }
                        }
                    }
                }

                OnPropertyChanged();
            }
        }

        public bool IsVisible => _isSearchVisible && _isConditionalVisible;

        public Setting(SettingMetadata metadata, SettingValue value)
            : this(metadata, ImmutableArray.Create(value))
        {
        }

        public Setting(SettingMetadata metadata, ImmutableArray<SettingValue> values)
        {
            Metadata = metadata;
            Values = values;
        }

        internal void Initialize(SettingContext context)
        {
            if (Context != null)
                throw new InvalidOperationException("Already initialized.");

            Context = context;

            if (context.HasConfigurableDimensions && Metadata.SupportsPerConfigurationValues)
            {
                var builder = ImmutableArray.CreateBuilder<object>();
                builder.Add(new SingleValueConfigurationCommand(this));
                builder.Add(new Separator());
                foreach (var (dimensionName, dimensionValues) in context.Dimensions)
                {
                    builder.Add(new DimensionConfigurationCommand(this, dimensionName, dimensionValues));
                }
                ConfigurationCommands = builder.ToImmutable();
            }
            else
            {
                ConfigurationCommands = ImmutableArray<object>.Empty;
            }
        }

        private void UpdateDependentVisibility(Setting target, object visibleWhenValue)
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

        public void AddDependentTarget(Setting target, object visibleWhenValue)
        {
            _dependentTargets ??= new List<(Setting target, object visibleWhenValue)>();

            _dependentTargets.Add((target, visibleWhenValue));

            UpdateDependentVisibility(target, visibleWhenValue);
        }

        public void UpdateSearchState(string searchString)
        {
            var wasVisible = IsVisible;

            _isSearchVisible = MatchesSearchText(searchString);

            if (wasVisible != IsVisible)
            {
                OnPropertyChanged(nameof(IsVisible));
            }

            bool MatchesSearchText(string searchString)
            {
                if (Name.IndexOf(searchString, StringComparison.CurrentCultureIgnoreCase) != -1)
                    return true;

                if (Description != null && Description.IndexOf(searchString, StringComparison.CurrentCultureIgnoreCase) != -1)
                    return true;

                foreach (var value in _values)
                {
                    foreach (var enumValue in value.EnumValues)
                    {
                        if (enumValue.IndexOf(searchString, StringComparison.CurrentCultureIgnoreCase) != -1)
                            return true;
                    }
                }

                foreach (var searchTerm in Metadata.SearchTerms)
                {
                    if (searchTerm.IndexOf(searchString, StringComparison.CurrentCultureIgnoreCase) != -1)
                        return true;
                }

                // TODO search evaluated/unevaluated values too

                return false;
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Setting Clone() => new Setting(Metadata, Values.Select(value => value.Clone()).ToImmutableArray());

        public override string ToString() => Identity.ToString();

        private sealed class SingleValueConfigurationCommand : ISettingConfigurationCommand
        {
            private readonly Setting _setting;

            public string Caption => "Use the same value across all configurations";

            public string? DimensionName => null;

            public SingleValueConfigurationCommand(Setting setting) => _setting = setting;

            public void Invoke()
            {
                if (!_setting.Values.IsEmpty)
                {
                    // Apply the first configured value to all configurations
                    // TODO consider showing UI when more than one value is available to choose between
                    _setting.Values = ImmutableArray.Create(new SettingValue(ImmutableDictionary<string, string>.Empty, _setting.Values.First().Value));
                }
            }
        }

        private sealed class DimensionConfigurationCommand : ISettingConfigurationCommand
        {
            private readonly Setting _setting;
            private readonly ImmutableArray<string> _dimensionValues;

            public string? DimensionName { get; }

            public string Caption => $"Vary value by {DimensionName}";

            public DimensionConfigurationCommand(Setting setting, string dimensionName, ImmutableArray<string> dimensionValues)
            {
                _setting = setting;
                DimensionName = dimensionName;
                _dimensionValues = dimensionValues;
            }

            public void Invoke()
            {
                // TODO expand/contract the Values array accordingly
            }
        }

        public static ICommand InvokeConfigurationCommand { get; } = new DelegateCommand<ISettingConfigurationCommand>(command => command.Invoke());
    }

    internal interface ISettingConfigurationCommand
    {
        string Caption { get; }
        
        string? DimensionName { get; }

        void Invoke();
    }
}
