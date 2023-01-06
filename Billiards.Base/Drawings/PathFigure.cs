using OpenCvSharp;

namespace Billiards.Base.Drawings;

public class PathFigure
{
    public bool IsClosed { get; set; }
    public Point2f StartPoint { get; set; }
    public LineSegmentCollection Segments { get; } = new ();
}