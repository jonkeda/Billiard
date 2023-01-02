using System.Collections.Generic;
using OpenCvSharp;

namespace Billiards.Wpf.Extensions
{
    public static class PointExtensions
    {
        public static System.Windows.Point? AsWindowsPoint(this Point2f? p)
        {
            if (p.HasValue)
            {
                return p.Value.AsWindowsPoint();
            }
            return null;
        }

        public static System.Windows.Point AsWindowsPoint(this Point2f p)
        {
            return new System.Windows.Point(p.X, p.Y);
        }

        public static List<System.Windows.Point> AsWindowsPoints(this IEnumerable<Point2f> points)
        {
            List<System.Windows.Point> newPoints = new ();
            foreach (Point2f point in points)
            {
                newPoints.Add(point.AsWindowsPoint());
            }
            return newPoints;
        }

    }
}
