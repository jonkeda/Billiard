namespace Billiards.Base.Extensions;

public static class FloatExtension
{
    private const float Epsilon = 1e-10f;

    public static bool IsZero(this float d)
    {
        return MathF.Abs(d) < Epsilon;
    }

}