using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Billiards.Wpf.UI.Converters;

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