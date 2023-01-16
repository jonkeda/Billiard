namespace Billiards.Base.Extensions;

public static class FloatExtension
{
    private const float Epsilon = 1e-10f;

    public static bool IsZero(this float d)
    {
        return MathF.Abs(d) < Epsilon;
    }

    public static float ToDegrees(this float value)
    {
        return (value * 180) / MathF.PI;
    }

    public static float ToRadians(this float degrees)
    {
        return (MathF.PI / 180) * degrees;
    }
}