using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Billiard.UI.Converters;

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