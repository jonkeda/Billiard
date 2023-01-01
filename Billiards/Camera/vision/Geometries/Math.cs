namespace Billiard.Camera.vision.Geometries;

public class Math
{
    public static float sqrt(float x)
    {
        return System.MathF.Sqrt(x);
    }

    public static float abs(float x)
    {
        return System.MathF.Abs(x);
    }

    public static float cos(float x)
    {
        return System.MathF.Cos(x);
    }

    public static float sin(float x)
    {
        return System.MathF.Sin(x);
    }

    public static float pow(float x, float y)
    {
        return System.MathF.Pow(x, y);
    }

    public static float max(float a, float b)
    {
        return System.Math.Max(a, b);
    }

    public static int max(int a, int b)
    {
        return System.Math.Max(a, b);
    }

    public static float min(float a, float b)
    {
        return System.Math.Min(a, b);
    }

}