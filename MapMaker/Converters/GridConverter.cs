using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using MapMaker.Controllers;

namespace MapMaker.Converters
{
    public class GridConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var settings = (SettingsController) Application.Current.FindResource(nameof(SettingsController));
            return Math.Round(System.Convert.ToDouble(value) / settings.Settings.GridCellWidth, 1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}