using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace MapMaker.Converters
{
    public class CollectionIndexConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var collection = (IList) values[0];
            var item = values[1];
            return collection.IndexOf(item);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}