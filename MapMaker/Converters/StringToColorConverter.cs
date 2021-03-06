using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MapMaker.Converters
{
    public class StringToColorConverter : IValueConverter
    {
        private static readonly ColorConverter _converter = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ColorConverter.ConvertFromString(value as string) ?? throw new ArgumentNullException(nameof(value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return _converter.ConvertToString(value) ?? throw new ArgumentNullException(nameof(value));
        }
    }
}