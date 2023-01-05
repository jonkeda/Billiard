using Billiards.Base.Drawings;

namespace Billiards.Wpf.Drawings
{
    public class WpfPlatformColor : IPlatformColor
    {
        public WpfPlatformColor(System.Windows.Media.Color color)
        {
            Color = color;
        }

        public System.Windows.Media.Color Color { get; set; }
    }
}
