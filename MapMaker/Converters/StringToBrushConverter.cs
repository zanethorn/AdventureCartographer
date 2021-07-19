using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MapMaker.Converters
{
    public class StringToBrushConverter:IValueConverter
    {
        private static readonly BrushConverter Converter = new BrushConverter();
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(System.Convert.ToString(value))
                ? Brushes.Transparent
                : Converter.ConvertFrom(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}