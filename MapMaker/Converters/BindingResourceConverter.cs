using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MapMaker.Converters
{
    public class BindingResourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var resource = Application.Current.FindResource(System.Convert.ToString(value));
            return resource ?? Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}