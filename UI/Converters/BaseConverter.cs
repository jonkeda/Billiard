using System;
using System.Collections;
using System.Globalization;

namespace VouwwandImages.UI.Converters
{
    public abstract class BaseConverter
    {
        #region IValueConverter Members

        protected bool DoConvert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            bool negated = false;
            string sParameter = parameter as string;
            if (sParameter != null
                && sParameter.StartsWith("!"))
            {
                negated = true;
                sParameter = sParameter.Substring(1);
            }
            bool result = false;
            if (value != null)
            {
                result = true;

                Type tParameter;
                IEnumerable iEnumerable;
                if (value is bool)
                {
                    result = (bool)value;
                }

                else if (value is Enum
                    || value is int)
                {
                    result = string.Compare(value.ToString(), sParameter, StringComparison.InvariantCulture) == 0;
                }

                else if ((tParameter = parameter as Type) != null)
                {
                    result = tParameter.IsInstanceOfType(value);
                }

                else if ((iEnumerable = value as IEnumerable) != null)
                {
                    result = iEnumerable.GetEnumerator().MoveNext();
                }

  
            }

            if (negated)
            {
                result = !result;
            }
            return result;
        }


        #endregion

    }
}