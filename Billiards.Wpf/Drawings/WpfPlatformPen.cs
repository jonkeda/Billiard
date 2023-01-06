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

public class WpfPlatformGeometry : IPlatformGeometry
{
    public WpfPlatformGeometry(System.Windows.Media.Geometry geometry)
    {
        Geometry = geometry;
    }

    public System.Windows.Media.Geometry Geometry { get; set; }
}

public class WpfPlatformFormattedText : IPlatformFormattedText
{
    public WpfPlatformFormattedText(System.Windows.Media.FormattedText formattedText)
    {
        FormattedText = formattedText;
    }

    public System.Windows.Media.FormattedText FormattedText { get; set; }
}

