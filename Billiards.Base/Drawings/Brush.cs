namespace Billiards.Base.Drawings;

public class Brush
{
    public Brush(Color color)
    {
        Color = color;
    }

    public Color? Color { get; set; }
    public IPlatformBrush? PlatformBrush { get; set; }
}