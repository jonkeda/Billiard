using System;
using System.Security.Policy;

namespace Utilities
{
    static class MathV
    {
        public static double Mix(double x, double y, double a)
        {
            return x * (1.0 - a) + y * a;
        }

        public static double Clamp(double x, double minVal, double maxVal)
        {
            return Math.Min(Math.Max(x, minVal), maxVal);
        }

        public static Vector2D PerpendicularA(this Vector2D v)
        {
            return new Vector2D(-v.y, v.x);
        }

        public static Vector2D PerpendicularB(this Vector2D v)
        {
            return new Vector2D(v.y, -v.x);
        }

        public static Vector2D Abs(Vector2D v)
        {
            return new Vector2D(Math.Abs(v.x), Math.Abs(v.y));
        }

        public static double Dot(Vector2D a, Vector2D b)
        {
            return a.x * b.x + a.y * b.y;
        }

        public static double NDot(Vector2D a, Vector2D b)
        {
            return a.x * b.x - a.y * b.y;
        }

        public static Vector2D Max(Vector2D v, double d)
        {
            return new Vector2D(Math.Max(v.x, d), Math.Max(v.y, d));
        }

        public static Vector2D Min(Vector2D v, float d)
        {
            return new Vector2D(Math.Min(v.x, d), Math.Min(v.y, d));
        }

        public static Vector2D Max(Vector2D a, Vector2D b)
        {
            return new Vector2D(
                Math.Max(a.x, b.x),
                Math.Max(a.y, b.y)
            );
        }

        public static Vector2D Min(Vector2D a, Vector2D b)
        {
            return new Vector2D(
                Math.Min(a.x, b.x),
                Math.Min(a.y, b.y)
            );
        }

        public static Vector2D GetVector(double angle)
        {
            angle = angle / 180 * Math.PI;

            return new Vector2D(Math.Cos(angle), Math.Sin(angle));
        }

        public static double GetAngle(this Vector2D a)
        {
            if (a.y > 0)
            {
                return Math.Acos(MathV.Dot(a, new Vector2D(1, 0))) * 180 / Math.PI;
            }
            else
            {
                return -Math.Acos(MathV.Dot(a, new Vector2D(1, 0))) * 180 / Math.PI;
            }
        }
    }
}
