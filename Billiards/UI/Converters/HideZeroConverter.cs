using System;
using System.Globalization;
using System.Windows.Data;

namespace Billiard.UI.Converters
{
    public class HideZeroConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int i)
            {
                if (i == 0)
                {
                    return "";
                }

                return i.ToString();
            }
            if (value is long l)
            {
                if (l == 0)
                {
                    return "";
                }

                return l.ToString();
            }
            if (value is double d)
            {
                if (d == 0)
                {
                    return "";
                }

                return d.ToString(CultureInfo.InvariantCulture);
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}