using System;
using System.Globalization;
using System.Windows.Data;

namespace MapMaker.Converters
{
    public class HalvingConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return 0.0;
            return System.Convert.ToDouble(value) / 2.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}