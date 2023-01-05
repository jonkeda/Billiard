using Billiards.Base.Drawings;

namespace Billiards.Wpf.Drawings;

public class WpfPlatformPen : IPlatformPen
{
    public WpfPlatformPen(System.Windows.Media.Pen pen)
    {
        Pen = pen;
    }

    public System.Windows.Media.Pen Pen { get; set; }
}