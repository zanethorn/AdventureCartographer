using System;
using System.Globalization;
using System.Windows.Data;

namespace MapMaker.Converters
{
    public class InvertDoubleConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var d = System.Convert.ToDouble(value);
            return 1.0 / d;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}