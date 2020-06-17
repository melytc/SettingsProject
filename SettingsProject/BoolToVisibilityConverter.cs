using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SettingsProject
{
    internal sealed class BoolToVisibilityConverter : IValueConverter
    {
        public static BoolToVisibilityConverter HiddenWhenFalse => new BoolToVisibilityConverter(Visibility.Visible, Visibility.Hidden);

        private readonly Visibility _trueVisibility;
        private readonly Visibility _falseVisibility;

        private BoolToVisibilityConverter(Visibility trueVisibility, Visibility falseVisibility)
        {
            _trueVisibility = trueVisibility;
            _falseVisibility = falseVisibility;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                return b ? _trueVisibility : _falseVisibility;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
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
