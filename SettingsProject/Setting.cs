using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

#nullable enable

namespace SettingsProject
{
    internal sealed class Setting : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly SettingMetadata _metadata;

        private bool _isSearchVisible = true;
        private bool _isConditionalVisible = true;
        private List<(Setting target, object visibleWhenValue)>? _dependentTargets;
        private ImmutableArray<SettingValue> _values;
        private SettingContext? _context;

        public string Name => _metadata.Name;
        public string Page => _metadata.Page;
        public string Category => _metadata.Category;
        public string? Description => _metadata.Description;
        public int Priority => _metadata.Priority;
        public SettingIdentity Identity => _metadata.Identity;
        public bool SupportsPerConfigurationValues => _metadata.SupportsPerConfigurationValues;

        public ISettingEditor? Editor { get; }
        public IReadOnlyDictionary<string, string> EditorMetadata { get; }

        public SettingContext Context => _context ?? throw new InvalidOperationException("Setting has not been initialized.");

        public ImmutableArray<SettingValue> Values
        {
            get => _values;
            set
            {
                // TODO validate incoming values
                // - set of dimension names across values must be identical
                // - number of values must match the dimension specifications

                _values = value;
                
                foreach (var settingValue in value)
                {
                    settingValue.Parent = this;
                    settingValue.PropertyChanged += OnSettingValuePropertyChanged;

                    void OnSettingValuePropertyChanged(object _, PropertyChangedEventArgs e)
                    {
                        if (e.PropertyName == nameof(SettingValue.EvaluatedValue) && _dependentTargets != null)
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
            _metadata = metadata;
            Values = values;

            (Editor, EditorMetadata) = SettingEditorFactory.Default.GetEditor(metadata.Editors);
        }

        internal void Initialize(SettingContext context)
        {
            if (_context != null)
                throw new InvalidOperationException("Already initialized.");

            _context = context;
        }

        private void UpdateDependentVisibility(Setting target, object visibleWhenValue)
        {
            var wasVisible = target.IsVisible;

            bool isConditionallyVisible = false;

            // Target is visible if any upstream value matches
            foreach (var value in Values)
            {
                if (Equals(visibleWhenValue, value.EvaluatedValue))
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

            _isSearchVisible = DoSearch();

            if (wasVisible != IsVisible)
            {
                OnPropertyChanged(nameof(IsVisible));
            }

            bool DoSearch()
            {
                if (IsMatch(Name))
                    return true;

                if (Description != null && IsMatch(Description))
                    return true;

                foreach (var value in _values)
                {
                    foreach (var enumValue in value.EnumValues)
                    {
                        if (IsMatch(enumValue))
                            return true;
                    }

                    if (IsMatch(value.UnevaluatedValue))
                        return true;

                    if (value.EvaluatedValue is string s && IsMatch(s))
                        return true;
                }

                foreach (var searchTerm in _metadata.SearchTerms)
                {
                    if (IsMatch(searchTerm))
                        return true;
                }

                return false;
            }

            bool IsMatch(string s) => s.IndexOf(searchString, StringComparison.CurrentCultureIgnoreCase) != -1;
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Setting Clone() => new Setting(_metadata, Values.Select(value => value.Clone()).ToImmutableArray());

        public override string ToString() => Identity.ToString();
    }

    internal interface ISettingConfigurationCommand
    {
        string Caption { get; }
        
        string? DimensionName { get; }

        ICommand Command { get; }
    }
}
