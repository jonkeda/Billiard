using Emgu.CV.Util;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using Billiard.Camera.vision.Geometries;
using Emgu.CV.Structure;

namespace Billiard.Models;

public interface IBoundingRectFilter : IAbstractFilter
{
    Rectangle BoundingRect { get; set; }
}

public class ContourCollection : Collection<Contour>
{

}

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

public interface IContourFilter : IAbstractFilter
{
    ContourCollection Contours { get; set; }
}