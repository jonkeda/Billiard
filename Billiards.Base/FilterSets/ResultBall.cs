using Billiards.Base.Filters;
using OpenCvSharp;

namespace Billiards.Base.FilterSets;

public class ResultBall
{
    public ResultBall(BallColor color, 
        Point2f? tableRelativePosition,
        Point2f? imageRelativePosition)
    {
        Color = color;
        TableRelativePosition = tableRelativePosition;
        ImageRelativePosition = imageRelativePosition;
    }

    public BallColor Color { get; set; }
    public Point2f? TableRelativePosition { get; set; }
    public Point2f? ImageRelativePosition { get; set; }
    public Point2f? ImageAbsolutePoint { get; set; }
}