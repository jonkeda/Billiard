namespace Billiards.Web.Shared;

public class Ball
{
    public Ball(BallColor color,
        Point? imagePoint, Point? tablePoint)
    {
        Color = color;
        ImagePoint = imagePoint;
        TablePoint = tablePoint;
    }

    public BallColor Color { get; }
    public Point? ImagePoint { get; }
    public Point? ImageAbsolutePoint { get; set; }
    public Point? TablePoint { get; }
    public Point? TableAbsolutePoint { get; set; }
}