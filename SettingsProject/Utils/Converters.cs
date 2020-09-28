using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

#nullable enable

namespace Microsoft.VisualStudio.ProjectSystem.VS.Implementation.PropertyPages.Designer
{
    internal static partial class Converters
    {
        public static IValueConverter HiddenWhenFalse    { get; } = new BoolToVisibilityConverter(trueVisibility: Visibility.Visible, falseVisibility: Visibility.Hidden);
        public static IValueConverter CollapsedWhenFalse { get; } = new BoolToVisibilityConverter(trueVisibility: Visibility.Visible, falseVisibility: Visibility.Collapsed);

        public static IValueConverter CollapsedWhenEmptyString    { get; } = new LambdaConverter<string, Visibility>(s => string.IsNullOrEmpty(s) ? Visibility.Collapsed : Visibility.Visible);
        public static IValueConverter CollapsedWhenNotEmptyString { get; } = new LambdaConverter<string, Visibility>(s => string.IsNullOrEmpty(s) ? Visibility.Visible : Visibility.Collapsed);

        public static IValueConverter DoubleToBottomThickness { get; } = new LambdaConverter<double, Thickness>(d => new Thickness(0, 0, 0, d));

        public static IValueConverter LinkActionHeading { get; } = new LambdaConverter<Property, string>(property => property.Description != null ? property.Name : "");
        public static IValueConverter LinkActionLinkText { get; } = new LambdaConverter<Property, string>(property => property.Description ?? property.Name);

        public static IMultiValueConverter DimensionNames { get; } = new LambdaMultiConverter<ImmutableDictionary<string, string>, ImmutableArray<string>, string>((map, dimensions) =>
        {
            if (map.IsEmpty)
                return "";

            if (map.Count == 1)
                return map.First().Value;

            var sb = new StringBuilder();

            foreach (var dimension in dimensions)
            {
                if (!map.TryGetValue(dimension, out string value))
                    continue;
                if (sb.Length != 0)
                    sb.Append(" & ");
                sb.Append(value);
            }

            return sb.ToString();
        });

        public static IMultiValueConverter DescriptionVisibility { get; } = new LambdaMultiConverter<Property, string, ImmutableArray<PropertyValue>, Visibility>(
            (property, description, values) => !string.IsNullOrWhiteSpace(description) && property.Editor?.ShouldShowDescription(values) != false ? Visibility.Visible : Visibility.Collapsed);

        public static IMultiValueConverter PropertyConfigurationCommandChecked { get; } = new LambdaMultiConverter<IPropertyConfigurationCommand, ImmutableArray<PropertyValue>, bool>(
            (command, values) =>
            {
                if (command.DimensionName == null)
                    return values.Length == 1;

                return values.Any(value => value.ConfigurationDimensions.Any(dim => dim.Key == command.DimensionName));
            });

        public static IMultiValueConverter PropertyConfigurationCommandEnabled { get; } = new LambdaMultiConverter<IPropertyConfigurationCommand, Property, bool>(
            (command, property) =>
            {
                if (command.DimensionName == null)
                    return true;

                return property.Context.Dimensions[command.DimensionName].Length > 1;
            });

        public static IValueConverter PropertyValueComboBoxViewModel { get; } = new LambdaConverter<PropertyValue, PropertyValueComboBoxViewModel>(
            propertyValue => new PropertyValueComboBoxViewModel(propertyValue));
    }
}
