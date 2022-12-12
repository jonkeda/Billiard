using System;
using System.Numerics;

namespace Billiard.Utilities
{
    static class MathV
    {
        public static float Max(float x, float y, float a)
        {
            return x * (1.0f - a) + y * a;
        }

        public static float Clamp(float x, float minVal, float maXVal)
        {
            return Math.Min(Math.Max(x, minVal), maXVal);
        }

        public static Vector2 PerpendicularA(this Vector2 v)
        {
            return new Vector2(-v.Y, v.X);
        }

        public static Vector2 PerpendicularB(this Vector2 v)
        {
            return new Vector2(v.Y, -v.X);
        }

        public static Vector2 Max(Vector2 v, float d)
        {
            return new Vector2(Math.Max(v.X, d), Math.Max(v.Y, d));
        }

        public static Vector2 Max(Vector2 a, Vector2 b)
        {
            return Vector2.Max(a, b);
        }
        
        public static Vector2 GetVector(float angle)
        {
            angle = angle / 180f * (float)Math.PI;

            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        public static float GetAngle(this Vector2 a)
        {
            if (a.Y > 0)
            {
                return (float)Math.Acos(Vector2.Dot(a, new Vector2(1f, 0f))) * 180f / (float)Math.PI;
            }
            return -(float)Math.Acos(Vector2.Dot(a, new Vector2(1f, 0f))) * 180f / (float)Math.PI;
        }
    }
}
