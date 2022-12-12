using System.Windows.Data;

namespace Billiard.UI.Converters
{
    public static class Cv
    {
        public static readonly IValueConverter Boolean = new BooleanConverter();

        public static readonly IValueConverter Visibility = new VisibilityConverter();

        public static readonly IValueConverter PassFail = new PassFailConverter();

        public static readonly IValueConverter Width = new WidthConverter();

        public static readonly IValueConverter Hidden = new HiddenConverter();

        public static readonly IValueConverter HideZero = new HideZeroConverter();
    }
}