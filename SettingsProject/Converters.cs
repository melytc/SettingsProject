using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

#nullable enable

namespace SettingsProject
{
    internal static class Converters
    {
//        public static IValueConverter HiddenWhenFalse    { get; } = new BoolToVisibilityConverter(trueVisibility: Visibility.Visible, falseVisibility: Visibility.Hidden);
        public static IValueConverter CollapsedWhenFalse { get; } = new BoolToVisibilityConverter(trueVisibility: Visibility.Visible, falseVisibility: Visibility.Collapsed);
//        public static IValueConverter CollapsedWhenTrue  { get; } = new BoolToVisibilityConverter(trueVisibility: Visibility.Collapsed, falseVisibility: Visibility.Visible);

        public static IValueConverter CollapsedWhenEmptyString   { get; } = new SingleValueVisibilityConverter(value: "", matchValue: Visibility.Collapsed, elseValue: Visibility.Visible);
        public static IValueConverter CollapsedWhenNull          { get; } = new SingleValueVisibilityConverter(value: null, matchValue: Visibility.Collapsed, elseValue: Visibility.Visible);

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
