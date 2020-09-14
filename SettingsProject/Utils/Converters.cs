using System.Collections.Immutable;
using System.Linq;
using System.Windows;
using System.Windows.Data;

#nullable enable

namespace SettingsProject
{
    internal static partial class Converters
    {
        public static IValueConverter HiddenWhenFalse    { get; } = new BoolToVisibilityConverter(trueVisibility: Visibility.Visible, falseVisibility: Visibility.Hidden);
        public static IValueConverter CollapsedWhenFalse { get; } = new BoolToVisibilityConverter(trueVisibility: Visibility.Visible, falseVisibility: Visibility.Collapsed);
        public static IValueConverter CollapsedWhenTrue  { get; } = new BoolToVisibilityConverter(trueVisibility: Visibility.Collapsed, falseVisibility: Visibility.Visible);

        public static IValueConverter CollapsedWhenEmptyString    { get; } = new LambdaConverter<string, Visibility>(s => string.IsNullOrEmpty(s) ? Visibility.Collapsed : Visibility.Visible);
        public static IValueConverter CollapsedWhenNotEmptyString { get; } = new LambdaConverter<string, Visibility>(s => string.IsNullOrEmpty(s) ? Visibility.Visible : Visibility.Collapsed);

        public static IValueConverter CollapsedWhenNull           { get; } = new SingleValueVisibilityConverter(value: null, matchValue: Visibility.Collapsed, elseValue: Visibility.Visible);

        public static IValueConverter DoubleToBottomThickness { get; } = new LambdaConverter<double, Thickness>(d => new Thickness(0, 0, 0, d));

        public static IValueConverter Negate { get; } = new LambdaConverter<bool, bool>(b => !b);

        public static IValueConverter LinkActionHeading { get; } = new LambdaConverter<Setting, string>(setting => setting.Description != null ? setting.Name : "");
        public static IValueConverter LinkActionLinkText { get; } = new LambdaConverter<Setting, string>(setting => setting.Description ?? setting.Name);

        public static IValueConverter DimensionNames { get; } = new LambdaConverter<ImmutableDictionary<string, string>, string>(dimensions => string.Join(" | ", dimensions.Values));

        public static IMultiValueConverter DescriptionVisibility { get; } = new LambdaMultiConverter<Setting, string, ImmutableArray<SettingValue>, Visibility>(
            (setting, description, values) => !string.IsNullOrWhiteSpace(description) && setting.Editor?.ShouldShowDescription(values) != false ? Visibility.Visible : Visibility.Collapsed);

        public static IMultiValueConverter SettingConfigurationCommandChecked { get; } = new LambdaMultiConverter<ISettingConfigurationCommand, ImmutableArray<SettingValue>, bool>(
            (command, values) =>
            {
                if (command.DimensionName == null)
                    return values.Length == 1;

                return values.Any(value => value.ConfigurationDimensions.Any(dim => dim.Key == command.DimensionName));
            });

        public static IMultiValueConverter SettingConfigurationCommandEnabled { get; } = new LambdaMultiConverter<ISettingConfigurationCommand, Setting, bool>(
            (command, setting) =>
            {
                if (command.DimensionName == null)
                    return true;

                return setting.Context.Dimensions[command.DimensionName].Length > 1;
            });
    }
}
