using OpenCvSharp;
using OpenCvSharp.Internal.Vectors;

namespace Billiards.Base.Filters;

public class Contour
{
    public Contour(VectorOfPoint contour)
    {
        List<Point> points = new List<Point>();
        points.AddRange(contour.ToArray());
        Points = points;
    }

    public Contour()
    { }

    public Contour(IEnumerable<Point> points)
    {
        Points = points.ToList();
    }

    public List<Point>? Points { get; set; }
    public RotatedRect? RotatedRectangle { get; set; }

/*    public Point2f[] AsArray()
    {
        return Points.ToArray();
    }
*/

}