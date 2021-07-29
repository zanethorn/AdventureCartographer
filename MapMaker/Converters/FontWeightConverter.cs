using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using FontWeight = MapMaker.Models.Map.FontWeight;

namespace MapMaker.Converters
{
    public class FontWeightConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (FontWeight) value switch
            {
                MapMaker.Models.Map.FontWeight.Thin => System.Windows.FontWeights.Thin,
                MapMaker.Models.Map.FontWeight.ExtraLight => System.Windows.FontWeights.ExtraLight,
                MapMaker.Models.Map.FontWeight.Light => System.Windows.FontWeights.Light,
                MapMaker.Models.Map.FontWeight.Normal => System.Windows.FontWeights.Normal,
                MapMaker.Models.Map.FontWeight.Medium => System.Windows.FontWeights.Medium,
                MapMaker.Models.Map.FontWeight.SemiBold => System.Windows.FontWeights.SemiBold,
                MapMaker.Models.Map.FontWeight.Bold => System.Windows.FontWeights.Bold,
                MapMaker.Models.Map.FontWeight.ExtraBold => System.Windows.FontWeights.ExtraBold,
                MapMaker.Models.Map.FontWeight.Black => System.Windows.FontWeights.Black,
                MapMaker.Models.Map.FontWeight.UltraBlack => System.Windows.FontWeights.UltraBlack,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}