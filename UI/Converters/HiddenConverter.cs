using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Billiard.UI.Converters
{
    public class HiddenConverter : BaseConverter, IValueConverter
    {
        public static readonly IValueConverter Instance = new VisibilityConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (DoConvert(value, targetType, parameter, culture))
            {
                return Visibility.Visible;
            }
            return Visibility.Hidden;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}