using System;
using System.Globalization;
using System.Windows.Data;

namespace MapMaker.Converters
{
    public class InvertDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var p = parameter == null ? 1.0 : System.Convert.ToDouble(parameter);
            var d = System.Convert.ToDouble(value);
            return p / d;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}