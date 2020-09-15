using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

#nullable disable

namespace SettingsProject
{
    internal static partial class Converters
    {
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
    }
}