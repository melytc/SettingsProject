using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

#nullable enable

namespace Microsoft.VisualStudio.ProjectSystem.VS.Implementation.PropertyPages.Designer
{
    internal sealed class PropertyContext
    {
        private readonly ImmutableArray<PropertyCondition> _propertyConditions;

        public ImmutableDictionary<string, ImmutableArray<string>> Dimensions { get; }

        public ImmutableArray<Property> Properties { get; }
        
        public ImmutableArray<object> ConfigurationCommands { get; }

        public ImmutableArray<string> DimensionOrder { get; }

        public PropertyContext(IReadOnlyList<KeyValuePair<string, ImmutableArray<string>>> dimensions, ImmutableArray<PropertyCondition> propertyConditions, ImmutableArray<Property> properties)
            : this(dimensions.ToImmutableDictionary(StringComparers.ConfigurationDimensionNames), dimensions.Select(pair => pair.Key).ToImmutableArray(), propertyConditions, properties)
        {
        }

        private PropertyContext(ImmutableDictionary<string, ImmutableArray<string>> dimensions, ImmutableArray<string> dimensionOrder, ImmutableArray<PropertyCondition> propertyConditions, ImmutableArray<Property> properties)
        {
            Dimensions = dimensions;
            DimensionOrder = dimensionOrder;
            _propertyConditions = propertyConditions;
            Properties = properties;

            var propertyByIdentity = properties.ToDictionary(property => property.Identity);

            foreach (var condition in propertyConditions)
            {
                if (!propertyByIdentity.TryGetValue(condition.Source, out Property source))
                    throw new Exception("Unknown source: " + condition.Source);
                if (!propertyByIdentity.TryGetValue(condition.Target, out Property target))
                    throw new Exception("Unknown target: " + condition.Target);

                if (source != null && target != null)
                {
                    source.AddDependentTarget(target, condition.SourceValue);
                }
            }

            foreach (var property in Properties)
            {
                property.Initialize(this);
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

        public PropertyContext Clone()
        {
            return new PropertyContext(
                Dimensions,
                DimensionOrder,
                _propertyConditions,
                Properties.Select(property => property.Clone()).ToImmutableArray());
        }

        private sealed class SingleValueConfigurationCommand : IPropertyConfigurationCommand
        {
            public string Caption => Resources.PropertyUseSameValueAcrossAllConfigurations;

            public string? DimensionName => null;

            public ICommand Command { get; }

            public SingleValueConfigurationCommand()
            {
                Command = new DelegateCommand<Property>(
                    property =>
                    {
                        if (!property.Values.IsEmpty)
                        {
                            // Apply the first configured value to all configurations
                            // TODO consider showing UI when more than one value is available to choose between
                            var value = property.Values.First();
                            property.Values = ImmutableArray.Create(new PropertyValue(value.UnevaluatedValue, value.EvaluatedValue));
                        }
                    });
            }
        }

        private sealed class DimensionConfigurationCommand : IPropertyConfigurationCommand
        {
            public string DimensionName { get; }

            public ICommand Command { get; }

            public string Caption => string.Format(Resources.PropertyVaryByDimension_1, DimensionName);

            public DimensionConfigurationCommand(string dimensionName, ImmutableArray<string> dimensionValues)
            {
                DimensionName = dimensionName;
                Command = new DelegateCommand<Property>(
                    property =>
                    {
                        bool isAdding = !property.Values.Any(value => value.ConfigurationDimensions.ContainsKey(dimensionName));

                        if (isAdding)
                        {
                            property.Values = property.Values
                                .SelectMany(value => dimensionValues.Select(dim => new PropertyValue(value.UnevaluatedValue, value.EvaluatedValue, value.ConfigurationDimensions.Add(dimensionName, dim))))
                                .ToImmutableArray();
                        }
                        else
                        {
                            Assumes.False(property.Values.IsEmpty);
                            var oldValueGroups = property.Values.GroupBy(value => value.ConfigurationDimensions.Remove(dimensionName), DimensionValueEqualityComparer.Instance);

                            property.Values = oldValueGroups
                                .Select(group => new PropertyValue(group.First().UnevaluatedValue, group.First().EvaluatedValue, group.First().ConfigurationDimensions.Remove(dimensionName)))
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