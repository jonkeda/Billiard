namespace Billiards.Base.Extensions;

public static class MathF2
{
    public static float DegreeDifference(float a, float b)
    {
        float dif = MathF.Abs(a - b);
        if (dif <= 180)
        {
            return dif;
        }

        return 360 - dif;
    }

    public static double toDegrees(double angrad)
    {
        return angrad * 57.29577951308232D;
    }

}