using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MapMaker.Converters
{
    public class ViewPortConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? new Rect() : new Rect(){Height = (int)value, Width=(int)value};
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}