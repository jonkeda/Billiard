using OpenCvSharp;

namespace Billiards.Base.Drawings;

public class LineShape : AbstractShape
{
    public Point2f P1 { get; }
    public Point2f P2 { get; }


    public LineShape(Pen pen, Point2f p1, Point2f p2) : base(null, pen)
    {
        P1 = p1;
        P2 = p2;
    }
}