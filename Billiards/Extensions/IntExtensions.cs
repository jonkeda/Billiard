using System;
using System.Globalization;

namespace Billiard.Extensions
{
    public static class IntExtensions
    {
        public static CultureInfo Culture = new CultureInfo("nl-NL");
        private const NumberStyles NumStyles = NumberStyles.AllowLeadingSign | NumberStyles.AllowThousands;

        public static bool Between(this int i, int min, int max)
        {
            if (i > min)
                return i < max;
            return false;
        }

        public static int ValOrZero(this int? i)
        {
            if (i.HasValue)
                return i.Value;
            return 0;
        }

        public static bool IsNullOrZero(this int? i)
        {
            if (!i.HasValue)
                return true;
            int? nullable = i;
            int num = 0;
            if (nullable.GetValueOrDefault() != num)
                return false;
            return nullable.HasValue;
        }

        public static int? Add(this int? i, int? j)
        {
            if (i.HasValue && j.HasValue)
            {
                int? nullable1 = i;
                int? nullable2 = j;
                if (!(nullable1.HasValue & nullable2.HasValue))
                    return new int?();
                return new int?(nullable1.GetValueOrDefault() + nullable2.GetValueOrDefault());
            }
            if (i.HasValue)
                return i;
            if (j.HasValue)
                return j;
            return new int?();
        }

        public static int? Substract(this int? i, int? j)
        {
            if (i.HasValue && j.HasValue)
            {
                int? nullable1 = i;
                int? nullable2 = j;
                if (!(nullable1.HasValue & nullable2.HasValue))
                    return new int?();
                return new int?(nullable1.GetValueOrDefault() - nullable2.GetValueOrDefault());
            }
            if (i.HasValue)
                return i;
            if (!j.HasValue)
                return new int?();
            int? nullable = j;
            if (!nullable.HasValue)
                return new int?();
            return new int?(-nullable.GetValueOrDefault());
        }

        public static int? TryParseNull(string val)
        {
            int? nullable = new int?();
            int result = 0;
            if (int.TryParse(val, NumberStyles.AllowLeadingSign | NumberStyles.AllowThousands, (IFormatProvider)Culture.NumberFormat, out result))
                nullable = new int?(result);
            return nullable;
        }

        public static int TryParse(string val)
        {
            int result = 0;
            int.TryParse(val, NumberStyles.AllowLeadingSign | NumberStyles.AllowThousands, (IFormatProvider)Culture.NumberFormat, out result);
            return result;
        }

        public static string ToString(this int? value, string format)
        {
            if (value.HasValue)
                return value.Value.ToString("N0", (IFormatProvider)Culture);
            return (string)null;
        }
    }
}
