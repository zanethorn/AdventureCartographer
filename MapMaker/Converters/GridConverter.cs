using System;
using System.Globalization;
using System.Windows.Data;
using MapMaker.Properties;

namespace MapMaker.Converters
{
    public class GridConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToDouble(value) / Settings.Default.GridCellWidth;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}