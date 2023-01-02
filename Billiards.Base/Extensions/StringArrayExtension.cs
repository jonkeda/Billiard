namespace Billiard.Extensions
{
    public static class StringArrayExtension
    {
        public static int GetInt32(this string[] values, int index)
        {
            if (values == null)
            {
                return -1;
            }
            if (index > values.Length)
            {
                return -1;
            }
            if (int.TryParse(values[index], out int result))
            {
                return result;
            }

            return -1;
        }

    }
}