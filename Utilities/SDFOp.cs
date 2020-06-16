using System;

namespace Utilities
{
    static class SDFOp
    {
        public static double Union(double d1, double d2) => Math.Min(d1, d2);
        public static double Subtraction(double d1, double d2) => Math.Max(-d1, d2);
        public static double Intersection(double d1, double d2) => Math.Max(d1, d2);
        public static double SmoothUnion(double d1, double d2, double k)
        {
            double h = MathV.Clamp(0.5 + 0.5 * (d2 - d1) / k, 0.0, 1.0);
            return MathV.Mix(d2, d1, h) - k * h * (1.0 - h);
        }
        public static double SmoothSubtraction(double d1, double d2, double k)
        {
            double h = MathV.Clamp(0.5f - 0.5f * (d2 + d1) / k, 0.0f, 1.0f);
            return MathV.Mix(d2, -d1, h) + k * h * (1.0f - h);
        }
        public static double SmoothIntersection(double d1, double d2, double k)
        {
            double h = MathV.Clamp(0.5 - 0.5 * (d2 - d1) / k, 0.0, 1.0);
            return MathV.Mix(d2, d1, h) + k * h * (1.0 - h);
        }

        public static double Negate(double d) => -d;

        // Numerical normal generation
        public static Vector2D GetNormal(Vector2D p, Func<Vector2D, double, bool, double> distanceFunction)
        {
            double h = 0.001f;

            Vector2D n = new Vector2D(
                distanceFunction(p + new Vector2D(h, 0), 0, true) - distanceFunction(p - new Vector2D(h, 0), 0, true),
                distanceFunction(p + new Vector2D(0, h), 0, true) - distanceFunction(p - new Vector2D(0, h), 0, true)
            );

            return n.Normalize();
        }
    }
}