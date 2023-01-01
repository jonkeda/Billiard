using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Billiard.Camera.vision.Geometries;
using Emgu.CV.Util;
using Math = Billiard.Camera.vision.Geometries.Math;

namespace Billiard.Camera.vision.algorithms
{
    public class GeometricMath
    {

        public static float distance(PointF p1, PointF p2)
        {
            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;
            return MathF.Sqrt(dx * dx + dy * dy);
        }

        public static float rectilinearDistance(PointF point1, PointF point2)
        {
            return Math.abs(point1.X - point2.X) + Math.abs(point1.Y - point2.Y);
        }

        public static float getRadius(List<PointF> points)
        {
            float maxDistance = 0;

            IEnumerator<PointF> pointIterator1 = points.GetEnumerator();
            while (pointIterator1.MoveNext())
            {
                PointF p1 = pointIterator1.Current;
                IEnumerator<PointF> pointIterator2 = pointIterator1;
                while (pointIterator2.MoveNext())
                {
                    PointF p2 = pointIterator2.Current;

                    maxDistance = Math.max(distance(p1, p2), maxDistance);

                }
            }

            return maxDistance;
        }

        public static float getRadiusOptimizedWithMaxValue(VectorOfPoint points, float maxRadius)
        {
            float maxDistance = 0;
            Point[] p = points.ToArray();
            IEnumerator pointIterator1 = p.GetEnumerator();
            while (pointIterator1.MoveNext())
            {
                Point p1 = (Point)pointIterator1.Current;
                IEnumerator pointIterator2 = pointIterator1;
                while (pointIterator2.MoveNext())
                {
                    Point p2 = (Point)pointIterator2.Current;
                    maxDistance = Math.max(distance(p1, p2), maxDistance);
                    if (maxDistance > maxRadius)
                        return maxDistance;
                }
            }

            return maxDistance;
        }

        public static PointF getGeometricAverage(List<PointF> points)
        {
            return new PointF(points.Sum(p => p.X) / points.size(), points.Sum(p => p.Y) / points.size());
        }

        public static Point getGeometricAverage(List<Point> points)
        {
            return new Point(points.Sum(p => p.X) / points.size(), points.Sum(p => p.Y) / points.size());
        }

        public static PointF getGeometricMedian(List<PointF> points)
        {
            float minSum = float.MaxValue;
            PointF geometricMedianPoint = new PointF(0, 0);
            foreach (PointF p in points)
            {
                float sum = 0;
                foreach (PointF pi in points)
                {
                    // TODO: optimointi mahdollisuus: tämä etäisyys lasketaan kaksi kertaa jokaiselle pisteelle.
                    // tosin tällä hetkellä optimointi ei ole oikein vaivan arvoista.
                    sum += Math.pow(p.X - pi.X, 2) + Math.pow(p.Y - pi.Y, 2);
                }
                if (sum < minSum)
                {
                    geometricMedianPoint = p;
                    sum = minSum;
                }
            }
            return geometricMedianPoint;
        }

    }
}
