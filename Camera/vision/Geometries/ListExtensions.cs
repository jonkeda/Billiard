using System.Collections.Generic;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Util;

namespace Billiard.Camera.vision.Geometries
{
    internal static class ListExtensions
    {
        public static int size<T>(this List<T> list)
        {
            return list.Count;
        }
    }


    internal static class VectorOfPointFExtensions
    {
        public static List<PointF> AsList(this VectorOfPointF vector)
        {
            List<PointF> points = new List<PointF>();
            foreach (PointF pointF in vector.ToArray())
            {
                points.Add(pointF);
            }
            return points;
        }
    }

    internal static class PointFExtensions
    {
        public static Point AsPoint(this PointF point)
        {
            return new Point((int) point.X, (int) point.Y);
        }
    }

    internal static class MatExtensions
    {
        public static float height(this Mat list)
        {
            return list.Height;
        }

        public static float width(this Mat list)
        {
            return list.Height;
        }

    }

}
