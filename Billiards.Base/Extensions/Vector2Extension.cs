using System.Numerics;

namespace Billiards.Base.Extensions;

public static class Vector2Extension
{
    public static bool IsZero(this Vector2 vector)
    {
        return vector.X == 0 && vector.Y == 0;
    }

    public static bool SmallerThen(this Vector2 a, Vector2 b) => a.X < b.X && a.Y < b.Y;
    // public static bool Great(Vector2 a, Vector2 b) => b < a;

}