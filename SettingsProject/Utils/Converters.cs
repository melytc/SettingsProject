using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

#nullable enable

namespace SettingsProject
{
    internal static class Converters
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
            (setting, description, values) => !string.IsNullOrWhiteSpace(description) && setting.Metadata.Editor?.ShouldShowDescription(values) != false ? Visibility.Visible : Visibility.Collapsed);

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

                return setting.Context!.Dimensions[command.DimensionName].Length > 1;
            });

        private sealed class LambdaConverter<TFrom, TTo> : IValueConverter
        {
            private readonly Func<TFrom, TTo> _convert;
            private readonly Func<TTo, TFrom>? _convertBack;

            public LambdaConverter(Func<TFrom, TTo> convert, Func<TTo, TFrom>? convertBack = null)
            {
                _convert = convert;
                _convertBack = convertBack;
            }

            public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is TFrom from)
                {
                    return _convert(from);
                }

                return value;
            }

            public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (_convertBack != null && value is TTo to)
                {
                    return _convertBack(to);
                }

                return value;
            }
        }

#nullable disable
        private sealed class LambdaMultiConverter<TFrom1, TFrom2, TTo> : IMultiValueConverter
        {
            private readonly Func<TFrom1, TFrom2, TTo> _convert;
            private readonly Func<TTo, (TFrom1, TFrom2)> _convertBack;

            public LambdaMultiConverter(Func<TFrom1, TFrom2, TTo> convert, Func<TTo, (TFrom1, TFrom2)> convertBack = null)
            {
                _convert = convert;
                _convertBack = convertBack;
            }

            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                if (values.Length == 2 && TryConvert(values[0], out TFrom1 t1) && TryConvert(values[1], out TFrom2 t2))
                {
                    return _convert(t1, t2);
                }

                return DependencyProperty.UnsetValue;

                static bool TryConvert<T>(object o, out T t)
                {
                    if (o is T tt)
                    {
                        t = tt;
                        return true;
                    }

                    t = default;
                    return o is null;
                }
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                if (_convertBack != null && value is TTo to)
                {
                    var values = _convertBack(to);
                    return new object[] { values.Item1, values.Item2 };
                }

                return new[] { DependencyProperty.UnsetValue, DependencyProperty.UnsetValue };
            }
        }

        private sealed class LambdaMultiConverter<TFrom1, TFrom2, TFrom3, TTo> : IMultiValueConverter
        {
            private readonly Func<TFrom1, TFrom2, TFrom3, TTo> _convert;
            private readonly Func<TTo, (TFrom1, TFrom2, TFrom3)> _convertBack;

            public LambdaMultiConverter(Func<TFrom1, TFrom2, TFrom3, TTo> convert, Func<TTo, (TFrom1, TFrom2, TFrom3)> convertBack = null)
            {
                _convert = convert;
                _convertBack = convertBack;
            }

            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                if (values.Length == 3 && TryConvert(values[0], out TFrom1 t1) && TryConvert(values[1], out TFrom2 t2) && TryConvert(values[2], out TFrom3 t3))
                {
                    return _convert(t1, t2, t3);
                }

                return DependencyProperty.UnsetValue;

                static bool TryConvert<T>(object o, out T t)
                {
                    if (o is T tt)
                    {
                        t = tt;
                        return true;
                    }

                    t = default;
                    return o is null;
                }
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                if (_convertBack != null && value is TTo to)
                {
                    var values = _convertBack(to);
                    return new object[] { values.Item1, values.Item2, values.Item3 };
                }

                return new[] { DependencyProperty.UnsetValue, DependencyProperty.UnsetValue, DependencyProperty.UnsetValue };
            }
        }
#nullable enable

        private sealed class SingleValueVisibilityConverter : IValueConverter
        {
            private readonly object? _value;
            private readonly object _matchValue;
            private readonly object _elseValue;

            public SingleValueVisibilityConverter(object? value, object matchValue, object elseValue)
            {
                _value = value;
                _matchValue = matchValue;
                _elseValue = elseValue;
            }

            public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            {
                return Equals(value, _value) ? _matchValue : _elseValue;
            }

            public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            {
                if (Equals(value, _matchValue))
                    return true;
                if (Equals(value, _elseValue))
                    return false;
                return value;
            }
        }

        private sealed class BoolToVisibilityConverter : IValueConverter
        {
            private readonly Visibility _trueVisibility;
            private readonly Visibility _falseVisibility;

            public BoolToVisibilityConverter(Visibility trueVisibility, Visibility falseVisibility)
            {
                _trueVisibility = trueVisibility;
                _falseVisibility = falseVisibility;
            }

            public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            {
                if (value is bool b)
                {
                    return b ? _trueVisibility : _falseVisibility;
                }

                return value;
            }

            public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            {
                if (value is Visibility v)
                {
                    if (v == _trueVisibility)
                    {
                        return true;
                    }

                    if (v == _falseVisibility)
                    {
                        return false;
                    }
                }

                return value;
            }
        }
    }
}
