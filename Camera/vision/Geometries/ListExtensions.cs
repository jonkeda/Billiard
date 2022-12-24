﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Windows.Media;
using Emgu.CV;
using Emgu.CV.Util;

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
        public static PointF Extend(PointF p1, PointF p2)
        {
            Vector2 v1 = p1.AsVector2();
            Vector2 v2 = p2.AsVector2();
            Vector2 x = (v1 - v2) * 1000 + v1;
            return x.AsPoint();
        }

        public static Vector2 AsVector2(this System.Windows.Point p)
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

        public static PointF AsPoint(this Vector2 point)
        {
            return new PointF(point.X, point.Y);
        }

        public static PointF AsPointF(this System.Windows.Point point)
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

    }

}
