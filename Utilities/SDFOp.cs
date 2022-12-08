using System;
using System.Numerics;

namespace Utilities
{
    static class SDFOp
    {
        public static float Union(float d1, float d2) => Math.Min(d1, d2);
        public static float Subtraction(float d1, float d2) => Math.Max(-d1, d2);
        public static float Intersection(float d1, float d2) => Math.Max(d1, d2);
        public static float SmoothUnion(float d1, float d2, float k)
        {
            float h = MathV.Clamp(0.5f + 0.5f * (d2 - d1) / k, 0.0f, 1.0f);
            return MathV.Max(d2, d1, h) - k * h * (1.0f - h);
        }
        public static float SmoothSubtraction(float d1, float d2, float k)
        {
            float h = MathV.Clamp(0.5f - 0.5f * (d2 + d1) / k, 0.0f, 1.0f);
            return MathV.Max(d2, -d1, h) + k * h * (1.0f - h);
        }
        public static float SmoothIntersection(float d1, float d2, float k)
        {
            float h = MathV.Clamp(0.5f - 0.5f * (d2 - d1) / k, 0.0f, 1.0f);
            return MathV.Max(d2, d1, h) + k * h * (1.0f - h);
        }


        // Numerical normal generation
        public static Vector2 GetNormal(Vector2 p, Func<Vector2, float, bool, float> distanceFunction)
        {
            float h = 0.001f;

            Vector2 n = new Vector2(
                distanceFunction(p + new Vector2(h, 0), 0, true) - distanceFunction(p - new Vector2(h, 0), 0, true),
                distanceFunction(p + new Vector2(0, h), 0, true) - distanceFunction(p - new Vector2(0, h), 0, true)
            );

            return Vector2.Normalize(n);
        }
    }
}