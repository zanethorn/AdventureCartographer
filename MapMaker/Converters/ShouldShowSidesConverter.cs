using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using MapMaker.Map;

namespace MapMaker.Converters
{
    public class ShouldShowSidesConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Visibility.Collapsed;
            var shapeType = (ShapeTypes)value;
            return shapeType == ShapeTypes.Polygon || shapeType == ShapeTypes.Star
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}