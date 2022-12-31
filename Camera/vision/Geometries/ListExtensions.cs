using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using Emgu.CV;
using Emgu.CV.Stitching;
using Emgu.CV.Util;
using Point = System.Drawing.Point;

namespace Billiard.Camera.vision.Geometries
{
    public static class ListExtensions
    {
        public static int size<T>(this List<T> list)
        {
            return list.Count;
        }
    }


    public static class VectorOfPointExtensions
    {
        public static List<Point> AsList(this VectorOfPoint vector)
        {
            List<Point> points = new List<Point>();
            foreach (Point point in vector.ToArray())
            {
                points.Add(point);
            }
            return points;
        }
    }

    public static class PointFExtensions
    {
        public static Vector2? Extend(System.Windows.Point? p1, System.Windows.Point? p2)
        {
            return Extend(p1, p2, 1000);
        }

        public static Vector2? Extend(Vector2 v1, Vector2 v2, int extender)
        {
            Vector2 x = (v1 - v2) * extender + v1;
            return x;
        }

        public static Vector2? Extend(System.Windows.Point? p1, System.Windows.Point? p2, int extender)
        {
            if (!p1.HasValue
                || !p2.HasValue)
            {
                return null;
            }
            return Extend(p1.Value.AsVector2(), p2.Value.AsVector2(), extender);
        }

        public static PointF Extend(PointF p1, PointF p2)
        {
            Vector2 v1 = p1.AsVector2();
            Vector2 v2 = p2.AsVector2();
            Vector2 x = (v1 - v2) * 1000 + v1;
            return x.AsPointF();
        }

        public static Vector2 AsVector2(this System.Windows.Point p)
        {
            return new Vector2((float)p.X, (float)p.Y);
        }

        public static Vector2 AsVector2(this System.Drawing.Point p)
        {
            return new Vector2((float)p.X, (float)p.Y);
        }

        public static Vector2 AsVector2(this PointF p)
        {
            return new Vector2(p.X, p.Y);
        }

        public static Vector2 AsVector2(this Point? p)
        {
            if (p == null)
            {
                return Vector2.Zero;
            }
            return new Vector2((float)p.Value.X, (float)p.Value.Y);
        }

        private const float Epsilon = 1e-10f;

        public static bool IsZero(this float d)
        {
            return MathF.Abs(d) < Epsilon;
        }

        public static float Cross(this Vector2 u, Vector2 v)
        {
            return u.X * v.Y - u.Y * v.X;
        }

        public static Point AsWindowsPoint(this PointF point)
        {
            return new Point((int) point.X, (int) point.Y);
        }

        public static System.Windows.Point AsPoint(this Vector2 point)
        {
            return new System.Windows.Point(point.X, point.Y);
        }

        public static PointF AsPointF(this Vector2 point)
        {
            return new PointF(point.X, point.Y);
        }

        public static PointF AsPointF(this System.Windows.Point point)
        {
            return new PointF((float)point.X, (float)point.Y);
        }
        public static PointF? AsPointF(this System.Windows.Point? point)
        {
            if (point.HasValue)
                return new PointF((float)point.Value.X, (float)point.Value.Y);
            return null;
        }

        public static PointF AsPointF(this System.Drawing.Point point)
        {
            return new PointF((float)point.X, (float)point.Y);
        }

        public static System.Windows.Point AsPoint(this PointF point)
        {
            return new System.Windows.Point(point.X, point.Y);
        }

        public static System.Windows.Point AsPoint(this System.Drawing.Point point)
        {
            return new System.Windows.Point(point.X, point.Y);
        }

        public static Point[] AsPointArray(this List<System.Drawing.Point> points)
        {
            Point[] a = new Point[points.Count];
            int i = 0;
            foreach (Point point in points)
            {
                a[i] = new Point(point.X, point.Y);
                i++;
            }
            return a;
        }

        public static PointF[] AsPointFArray(this List<System.Drawing.Point> points)
        {
            PointF[] a = new PointF[points.Count];
            int i = 0;
            foreach (Point point in points)
            {
                a[i] = point.AsPointF();
                i++;
            }
            return a;
        }

        public static List<System.Drawing.PointF> AsListOfPointF(this IEnumerable<System.Windows.Point> points)
        {
            List<System.Drawing.PointF> newPoints = new();
            int i = 0;
            foreach (System.Windows.Point point in points)
            {
                newPoints.Add(new PointF((int)point.X, (int)point.Y));
                i++;
            }
            return newPoints;
        }

        public static List<System.Windows.Point> AsListOfPoint(this IEnumerable<System.Drawing.PointF> points)
        {
            List<System.Windows.Point> newPoints = new();
            int i = 0;
            foreach (PointF point in points)
            {
                newPoints.Add(new System.Windows.Point((int)point.X, (int)point.Y));
                i++;
            }
            return newPoints;
        }


        public static List<System.Drawing.Point> AsListOfDrawingPoint(this IEnumerable<System.Drawing.PointF> points)
        {
            List<System.Drawing.Point> newPoints = new();
            int i = 0;
            foreach (PointF point in points)
            {
                newPoints.Add(new Point((int)point.X, (int)point.Y));
                i++;
            }
            return newPoints;
        }


        public static bool LineSegmentsIntersect(Vector2? pn, Vector2? p2n, Vector2? qn, Vector2? q2n,
        out Vector2 intersection, bool considerCollinearOverlapAsIntersect = false)
        {
            intersection = new Vector2();

            if (!pn.HasValue
                || !p2n.HasValue
                || !qn.HasValue
                || !q2n.HasValue)
            {
                return false;
            }
            Vector2 p = pn.Value;
            Vector2 p2 = p2n.Value;
            Vector2 q = qn.Value;
            Vector2 q2 = q2n.Value;

            Vector2 r = p2 - p;
            Vector2 s = q2 - q;
            float rxs = r.Cross(s);
            float qpxr = (q - p).Cross(r);

            // If r x s = 0 and (q - p) x r = 0, then the two lines are collinear.
            if (rxs.IsZero() && qpxr.IsZero())
            {
                // 1. If either  0 <= (q - p) * r <= r * r or 0 <= (p - q) * s <= * s
                // then the two lines are overlapping,
                /*                if (considerCollinearOverlapAsIntersect)
                                    if ((0 <= (q - p) * r 
                                         && (q - p) * r <= r * r) 
                                        || (0 <= (p - q) * s && (p - q) * s <= s * s))
                                        return true;
                */
                // 2. If neither 0 <= (q - p) * r = r * r nor 0 <= (p - q) * s <= s * s
                // then the two lines are collinear but disjoint.
                // No need to implement this expression, as it follows from the expression above.
                return false;
            }

            // 3. If r x s = 0 and (q - p) x r != 0, then the two lines are parallel and non-intersecting.
            if (rxs.IsZero() && !qpxr.IsZero())
                return false;

            // t = (q - p) x s / (r x s)
            var t = (q - p).Cross(s) / rxs;

            // u = (q - p) x r / (r x s)

            var u = (q - p).Cross(r) / rxs;

            // 4. If r x s != 0 and 0 <= t <= 1 and 0 <= u <= 1
            // the two line segments meet at the point p + t r = q + u s.
            if (!rxs.IsZero() && (0 <= t && t <= 1) && (0 <= u && u <= 1))
            {
                // We can calculate the intersection point using either t or u.
                intersection = p + t * r;

                // An intersection was found.
                return true;
            }

            // 5. Otherwise, the two line segments are not parallel but do not intersect.
            return false;
        }

        public static bool LineIntersect(System.Windows.Point? left1, System.Windows.Point? left2, 
            System.Windows.Point? top1, 
            System.Windows.Point? top2, out Vector2 intersection)
        {
            Vector2? eLeft1 = PointFExtensions.Extend(left1, left2, 10);
            Vector2? eLeft2 = PointFExtensions.Extend(left2, left1, 10);

            Vector2? eTop1 = PointFExtensions.Extend(top1, top2, 10);
            Vector2? eTop2 = PointFExtensions.Extend(top2, top1, 10);

            return PointFExtensions.LineSegmentsIntersect(eTop1, eTop2, eLeft1, eLeft2, out intersection);
        }

        public static bool LineIntersect(Vector2 left1, Vector2 left2,
            Vector2 top1, Vector2 top2, 
            out Vector2 intersection)
        {
            Vector2? eLeft1 = PointFExtensions.Extend(left1, left2, 10);
            Vector2? eLeft2 = PointFExtensions.Extend(left2, left1, 10);

            Vector2? eTop1 = PointFExtensions.Extend(top1, top2, 10);
            Vector2? eTop2 = PointFExtensions.Extend(top2, top1, 10);

            return PointFExtensions.LineSegmentsIntersect(eTop1, eTop2, eLeft1, eLeft2, out intersection);
        }

    }

    public static class RectangleExtensions
    {
        public static System.Windows.Rect AsRect(this System.Drawing.Rectangle r)
        {
            return new Rect(new System.Windows.Point(r.X, r.Y), new System.Windows.Size(r.Size.Width, r.Size.Height));
        }
    }

    public static class MatExtensions
    {
        public static ImageSource ToImageSource(this Mat mat)
        {
            if (mat == null
                || mat.IsEmpty)
            {
                return null;
            }
            return mat.ToBitmapSource();
        }

        public static float height(this Mat list)
        {
            return list.Height;
        }

        public static float width(this Mat list)
        {
            return list.Height;
        }

        public static void SetDoubleValue(this Mat mat, int row, int col, double value)
        {
            var target = new[] { value };
            Marshal.Copy(target, 0, mat.DataPointer + (row * mat.Cols + col) * mat.ElementSize, 1);
        }


        public static double GetDoubleValue(this Mat mat, int row, int col)
        {
            var value = new double[1];
            Marshal.Copy(mat.DataPointer + (row * mat.Cols + col) * mat.ElementSize, value, 0, 1);
            return value[0];
        }

        public static void SetFloatValue(this Mat mat, int row, int col, float value)
        {
            var target = new[] { value };
            Marshal.Copy(target, 0, mat.DataPointer + (row * mat.Cols + col) * mat.ElementSize, 1);
        }


        public static float GetFloatValue(this Mat mat, int row, int col)
        {
            var value = new float[1];
            Marshal.Copy(mat.DataPointer + (row * mat.Cols + col) * mat.ElementSize, value, 0, 1);
            return value[0];
        }

    }

}
