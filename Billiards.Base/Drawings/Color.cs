namespace Billiards.Base.Drawings;

public class Color
{
    public static Color FromArgb(int a, int r, int g, int b)
    {
        return new Color();
    }

    public IPlatformColor? PlatformColor { get; set; }
}