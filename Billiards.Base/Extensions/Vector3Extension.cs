using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Billiards.Base.Extensions
{
    public static class Vector3Extension
    {
        public static Vector3 normalize(this Vector3 v)
        {
            return Vector3.Normalize(v);
        }

        public static Vector3 cross(this Vector3 v1, Vector3 v2)
        {
            return Vector3.Cross(v1, v2);
        }

        public static Vector3 mult(this Vector3 v1, Vector3 v2)
        {
            return Vector3.Multiply(v1, v2);
        }

        public static Vector3 mult(this Vector3 v1, float v2)
        {
            return Vector3.Multiply(v1, v2);
        }

        public static Vector3 subtract(this Vector3 v1, Vector3 v2)
        {
            return Vector3.Subtract(v1, v2);
        }

        public static Vector3 add(this Vector3 v1, Vector3 v2)
        {
            return Vector3.Add(v1, v2);
        }

        public static Vector3 clone(this Vector3 v1)
        {
            return new Vector3(v1.X, v1.Y, v1.Z);
        }

    }
}
