using OpenCvSharp;
using OpenCvSharp.Internal.Vectors;

namespace Billiards.Base.Filters;

public class Contour
{
    public Contour(VectorOfPoint contour, int index)
    {
        List<Point> points = new List<Point>();
        points.AddRange(contour.ToArray());
        Points = points;
        Index = index;
    }

    public Contour()
    { }

    public Contour(IEnumerable<Point> points, int index, RotatedRect? rotatedRectangle)
    {
        Points = points.ToList();
        Index = index;
        RotatedRectangle = rotatedRectangle;
    }

    public int Index { get; set; }
    public List<Point>? Points { get; set; }
    public RotatedRect? RotatedRectangle { get; set; }

/*    public Point2f[] AsArray()
    {
        return Points.ToArray();
    }
*/

}