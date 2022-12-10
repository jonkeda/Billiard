using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace VouwwandImages.UI.Converters
{
    public class PassFailConverter : BaseConverter, IValueConverter
    {
        public static readonly IValueConverter Instance = new PassFailConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (DoConvert(value, targetType, parameter, culture))
            {
                return Brushes.Green;
            }
            return Brushes.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class WidthConverter : BaseConverter, IValueConverter
    {
        public static readonly IValueConverter Instance = new PassFailConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (DoConvert(value, targetType, parameter, culture))
            {
                return null;
            }
            return new GridLength(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }


    public class VisibilityConverter : BaseConverter, IValueConverter
    {
        public static readonly IValueConverter Instance = new VisibilityConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (DoConvert(value, targetType, parameter, culture))
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}