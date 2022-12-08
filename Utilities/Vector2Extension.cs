using System.Numerics;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Billiard.Utilities
{
    static class Vector2Extension
    {
        public static bool Zero(this Vector2 vector)
        {
             return vector.X == 0 && vector.Y == 0;
        }

        public static Vector2 Normalize(this Vector2 vector)
        {
            return Vector2.Normalize(vector);
        }

        public static Point AsPoint(this Vector2 vector)
        {
            return new Point(vector.X, vector.Y);
        }

        public static Point3D AsPoint3D(this Vector2 p) => new Point3D(p.X, p.Y, 0);

        public static bool SmallerThen(this Vector2 a, Vector2 b) => a.X < b.X && a.Y < b.Y;
        // public static bool Great(Vector2 a, Vector2 b) => b < a;

    }
}
