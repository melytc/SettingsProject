using System;
using System.Globalization;
using System.Windows.Data;

#nullable enable

namespace SettingsProject
{
    internal static partial class Converters
    {
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
    }
}