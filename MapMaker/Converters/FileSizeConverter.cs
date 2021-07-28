using System;
using System.Globalization;
using System.Windows.Data;

namespace MapMaker.Converters
{
    public class FileSizeConverter : IValueConverter
    {
        private static readonly string[] SizeSuffixes =
            {"bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"};

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = System.Convert.ToInt64(value);
            if (v == 0) return $"{v:n1} bytes";

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            var mag = (int) Math.Log(v, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            var adjustedSize = (decimal) v / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, 1) < 1000)
                return $"{adjustedSize:n1} {SizeSuffixes[mag]}";
            mag += 1;
            adjustedSize /= 1024;

            return $"{adjustedSize:n1} {SizeSuffixes[mag]}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}