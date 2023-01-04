using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Billiards.Wpf.UI.Converters
{
    public class BooleanConverter : BaseConverter, IValueConverter
    {
        public static readonly IValueConverter Instance = new BooleanConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DoConvert(value, targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string parameterString = parameter as string;
            if (parameterString == null)
            {
                return DependencyProperty.UnsetValue;
            }

            if (targetType.IsGenericType
                && targetType.GenericTypeArguments.Length > 0)
            {
                return Enum.Parse(targetType.GenericTypeArguments[0], parameterString);
            }

            return Enum.Parse(targetType, parameterString);
        }
    }
}