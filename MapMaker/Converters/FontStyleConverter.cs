using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using FontStyle = MapMaker.Models.Map.FontStyle;

namespace MapMaker.Converters
{
    public class FontStyleConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (FontStyle) value switch
            {
                FontStyle.Normal => FontStyles.Normal,
                FontStyle.Italic => FontStyles.Italic,
                FontStyle.Oblique => FontStyle.Oblique,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}