using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Billiard.Camera.vision.Geometries;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace Billiard.Models;

public class Contour
{
    public Contour(VectorOfPoint contour)
    {
        List<System.Drawing.Point> points = new List<Point>();
        for (int i = 0; i < contour.Size; i++)
        {
            points.Add(contour[i]);
        }

        Points = points;
    }

    public Contour()
    { }

    public Contour(IEnumerable<System.Drawing.Point> points)
    {
        Points = points.ToList();
    }

    public List<System.Drawing.Point> Points { get; set; }
    public RotatedRect? RotatedRectangle { get; set; }

    public VectorOfPoint AsVectorOfPoint()
    {
        return new VectorOfPoint(Points.ToArray());
    }

    public PointF[] AsArray()
    {
        PointF[] a = new PointF[Points.Count];
        int i = 0;
        foreach (Point point in Points)
        {
            a[i] = point.AsPointF();
            i++;
        }
        return a;
    }


}