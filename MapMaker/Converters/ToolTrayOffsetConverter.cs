using System;
using System.Globalization;
using System.Windows.Data;

namespace MapMaker.Converters
{
    public class ToolTrayOffsetConverter:IMultiValueConverter  
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var o = (parameter == null) ? 1.0 : System.Convert.ToDouble(parameter);
            var x = System.Convert.ToDouble(values[0]);
            var s = System.Convert.ToDouble(values[1]);
            return x-(o / s);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}