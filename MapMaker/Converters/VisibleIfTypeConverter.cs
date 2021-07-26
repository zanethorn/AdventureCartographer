using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MapMaker.Converters
{
    public class VisibleIfTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Visibility.Collapsed;
            return ((Type) parameter).IsInstanceOfType(value)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}