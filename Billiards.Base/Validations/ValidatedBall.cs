using Billiards.Base.Filters;

namespace Billiards.Base.Validations;

public class ValidatedBall
{
    public ValidatedBall()
    {

    }

    public ValidatedBall(BallColor color)
    {
        Color = color;
    }

    public BallColor Color { get; set; }
    public int X { get; set;}
    public int Y { get; set; }
    public int ResultX { get; set; }
    public int ResultY { get; set; }
}