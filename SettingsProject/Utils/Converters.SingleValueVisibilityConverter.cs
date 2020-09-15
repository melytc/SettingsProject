using System;
using System.Globalization;
using System.Windows.Data;

#nullable enable

namespace SettingsProject
{
    internal static partial class Converters
    {
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
    }
}