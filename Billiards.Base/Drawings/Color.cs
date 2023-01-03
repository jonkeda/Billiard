namespace Billiards.Base.Drawings;

public class Color
{
    public int A { get; }
    public int R { get; }
    public int G { get; }
    public int B { get; }

    public Color(int a, int r, int g, int b)
    {
        A = a;
        R = r;
        G = g;
        B = b;
    }

    public static Color FromRgb(int r, int g, int b)
    {
        return new Color(255, r, g, b);
    }

    public static Color FromArgb(int a, int r, int g, int b)
    {
        return new Color(a, r, g, b);
    }

    public IPlatformColor? PlatformColor { get; set; }
}