using OpenCvSharp;

namespace Billiards.Base.Drawings;

public class LineSegment
{
    public Point2f Point { get; }
    public bool IsStroked { get; }

    public LineSegment(Point2f point, bool isStroked)
    {
        Point = point;
        IsStroked = isStroked;
    }
}