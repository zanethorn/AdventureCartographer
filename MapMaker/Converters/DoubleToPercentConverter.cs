using System;
using System.Globalization;
using System.Windows.Data;

namespace MapMaker.Converters
{
    public class DoubleToPercentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var digits = parameter == null ? 1 : System.Convert.ToInt32(parameter);
            var d = value == null ? 0.0 : System.Convert.ToDouble(value);
            return d.ToString($"P{digits}");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}