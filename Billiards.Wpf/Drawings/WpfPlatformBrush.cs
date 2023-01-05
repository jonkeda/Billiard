using Billiards.Base.Drawings;

namespace Billiards.Wpf.Drawings;

public class WpfPlatformBrush : IPlatformBrush
{
    public WpfPlatformBrush(System.Windows.Media.Brush brush)
    {
        Brush = brush;
    }

    public System.Windows.Media.Brush Brush { get; set; }

}