using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft;

#nullable enable

namespace SettingsProject
{
    internal sealed class SettingContext
    {
        private readonly ImmutableArray<SettingCondition> _settingConditions;

        public IImmutableDictionary<string, ImmutableArray<string>> Dimensions { get; }

        public ImmutableArray<Setting> Settings { get; }
        
        public ImmutableArray<object> ConfigurationCommands { get; }

        public SettingContext(IImmutableDictionary<string, ImmutableArray<string>> dimensions, ImmutableArray<SettingCondition> settingConditions, ImmutableArray<Setting> settings)
        {
            _settingConditions = settingConditions;
            Dimensions = dimensions;
            Settings = settings;

            var settingByIdentity = settings.ToDictionary(setting => setting.Identity);

            foreach (var condition in settingConditions)
            {
                if (!settingByIdentity.TryGetValue(condition.Source, out Setting source))
                    throw new Exception("Unknown source: " + condition.Source);
                if (!settingByIdentity.TryGetValue(condition.Target, out Setting target))
                    throw new Exception("Unknown target: " + condition.Target);

                if (source != null && target != null)
                {
                    source.AddDependentTarget(target, condition.SourceValue);
                }
            }

            foreach (var setting in Settings)
            {
                setting.Initialize(this);
            }

            var hasConfigurableDimension = dimensions.Any(entry => entry.Value.Length > 1);

            if (hasConfigurableDimension)
            {
                var builder = ImmutableArray.CreateBuilder<object>();
                builder.Add(new SingleValueConfigurationCommand());
                builder.Add(new Separator());
                foreach (var (dimensionName, dimensionValues) in Dimensions)
                {
                    builder.Add(new DimensionConfigurationCommand(dimensionName, dimensionValues));
                }
                ConfigurationCommands = builder.ToImmutable();
            }
            else
            {
                ConfigurationCommands = ImmutableArray<object>.Empty;
            }
        }

        public SettingContext Clone()
        {
            return new SettingContext(
                Dimensions,
                _settingConditions,
                Settings.Select(setting => setting.Clone()).ToImmutableArray());
        }

        private sealed class SingleValueConfigurationCommand : ISettingConfigurationCommand
        {
            public string Caption => "Use the same value across all configurations";

            public string? DimensionName => null;

            public ICommand Command { get; }

            public SingleValueConfigurationCommand()
            {
                Command = new DelegateCommand<Setting>(
                    setting =>
                    {
                        if (!setting.Values.IsEmpty)
                        {
                            // Apply the first configured value to all configurations
                            // TODO consider showing UI when more than one value is available to choose between
                            var value = setting.Values.First();
                            setting.Values = ImmutableArray.Create(new SettingValue(value.UnevaluatedValue, value.EvaluatedValue));
                        }
                    });
            }
        }

        private sealed class DimensionConfigurationCommand : ISettingConfigurationCommand
        {
            public string DimensionName { get; }

            public ICommand Command { get; }

            public string Caption => $"Vary value by {DimensionName}";

            public DimensionConfigurationCommand(string dimensionName, ImmutableArray<string> dimensionValues)
            {
                DimensionName = dimensionName;
                Command = new DelegateCommand<Setting>(
                    setting =>
                    {
                        bool isAdding = !setting.Values.Any(value => value.ConfigurationDimensions.ContainsKey(dimensionName));

                        if (isAdding)
                        {
                            setting.Values = setting.Values
                                .SelectMany(value => dimensionValues.Select(dim => new SettingValue(value.UnevaluatedValue, value.EvaluatedValue, value.ConfigurationDimensions.Add(dimensionName, dim))))
                                .ToImmutableArray();
                        }
                        else
                        {
                            Assumes.False(setting.Values.IsEmpty);
                            var oldValueGroups = setting.Values.GroupBy(value => value.ConfigurationDimensions.Remove(dimensionName), DimensionValueEqualityComparer.Instance);

                            setting.Values = oldValueGroups
                                .Select(group => new SettingValue(group.First().UnevaluatedValue, group.First().EvaluatedValue, group.First().ConfigurationDimensions.Remove(dimensionName)))
                                .ToImmutableArray();
                        }
                    });
            }

            private sealed class DimensionValueEqualityComparer : IEqualityComparer<ImmutableDictionary<string, string>>
            {
                public static DimensionValueEqualityComparer Instance { get; } = new DimensionValueEqualityComparer();

                public bool Equals(ImmutableDictionary<string, string> x, ImmutableDictionary<string, string> y)
                {
                    if (x.Count != y.Count)
                        return false;

                    foreach (var (key, a) in x)
                    {
                        if (!y.TryGetValue(key, out var b))
                            return false;
                        if (!StringComparers.ConfigurationDimensionValues.Equals(a, b))
                            return false;
                    }

                    return true;
                }

                public int GetHashCode(ImmutableDictionary<string, string> obj)
                {
                    var hashCode = 1;
                    foreach (var (key, value) in obj)
                    {
                        hashCode = (hashCode * 397) ^ StringComparers.ConfigurationDimensionNames.GetHashCode(key);
                        hashCode = (hashCode * 397) ^ StringComparers.ConfigurationDimensionValues.GetHashCode(value);
                    }
                    return hashCode;
                }
            }
        }
    }
}