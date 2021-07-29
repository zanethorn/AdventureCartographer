using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using FontStretch = MapMaker.Models.Map.FontStretch;

namespace MapMaker.Converters
{
    public class FontStretchConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (FontStretch) value switch
            {
                FontStretch.UltraCondensed => FontStretches.UltraCondensed,
                FontStretch.ExtraCondensed => FontStretches.ExtraCondensed,
                FontStretch.Condensed => FontStretches.Condensed,
                FontStretch.SemiCondensed => FontStretches.SemiCondensed,
                FontStretch.Normal => FontStretches.Normal,
                FontStretch.SemiExpanded => FontStretches.SemiExpanded,
                FontStretch.Expanded => FontStretches.Expanded,
                FontStretch.ExtraExpanded => FontStretches.ExtraExpanded,
                FontStretch.UltraExpanded => FontStretches.UltraExpanded,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}