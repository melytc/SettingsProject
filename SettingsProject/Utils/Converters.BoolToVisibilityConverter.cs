using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

#nullable enable

namespace Microsoft.VisualStudio.ProjectSystem.VS.Implementation.PropertyPages.Designer
{
    internal static partial class Converters
    {
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