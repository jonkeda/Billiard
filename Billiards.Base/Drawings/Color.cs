namespace Billiards.Base.Drawings;

public class Color
{
    public byte A { get; }
    public byte R { get; }
    public byte G { get; }
    public byte B { get; }

    public Color(byte a, byte r, byte g, byte b)
    {
        A = a;
        R = r;
        G = g;
        B = b;
    }

    public static Color FromRgb(byte r, byte g, byte b)
    {
        return new Color(255, r, g, b);
    }

    public static Color FromArgb(byte a, byte r, byte g, byte b)
    {
        return new Color(a, r, g, b);
    }

    public IPlatformColor? PlatformColor { get; set; }
}