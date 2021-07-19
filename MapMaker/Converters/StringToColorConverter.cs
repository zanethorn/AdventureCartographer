using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MapMaker.Converters
{
    public class StringToColorConverter:IValueConverter
    {
        private static readonly ColorConverter _converter = new ColorConverter();
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ColorConverter.ConvertFromString((string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return _converter.ConvertToString(value);
        }
    }
}