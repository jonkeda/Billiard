using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using OpenCvSharp;

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


public class Mat2ImageSourceConverter : BaseConverter, IValueConverter
{
    public static readonly IValueConverter Instance = new PassFailConverter();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        try
        {
            if (value == null
                || ((Mat)value).Data == 0)
            {
                return null;
            }

            return OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToBitmapSource((Mat)value);

        }
        catch  
        {
            //
        }

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }
}