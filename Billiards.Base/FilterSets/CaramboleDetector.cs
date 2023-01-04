using Billiards.Base.Filters;
using OpenCvSharp;

namespace Billiards.Base.FilterSets;

public class CaramboleDetector
{
    public CaramboleDetector()
    {
        FilterSets.Clear();

        var table = FilterSets.AddSet(new TableDetectorSet());
        CornerDetectorSet corner = FilterSets.AddSet(new CornerDetectorSet(table.OriginalFilter, table.FloodFilter));
        var ball = FilterSets.AddSet(new BallDetectorSet(corner.ResultFilter()));
        BallResultFilter = ball.BallResultFilter;
        PointsFilter = corner.PointsFilter;
    }

    public BallResultFilter BallResultFilter { get; }
    public IPointsFilter PointsFilter { get; set; }
    public FilterSetCollection FilterSets { get; } = new();

    public void ApplyFilters(ResultModel result)
    {
        FilterSets.ApplyFilters(result.Image);

        result.Corners = PointsFilter.Points;
        result.WhiteBallPoint = BallResultFilter.WhiteBallPoint;
        result.YellowBallPoint = BallResultFilter.YellowBallPoint;
        result.RedBallPoint = BallResultFilter.RedBallPoint;

        result.Balls.Add(new ResultBall(BallColor.White, 
            ToRelativePoint(result.Image, BallResultFilter.WhiteBallPoint)));
        result.Balls.Add(new ResultBall(BallColor.Yellow,
            ToRelativePoint(result.Image, BallResultFilter.YellowBallPoint)));
        result.Balls.Add(new ResultBall(BallColor.Red,
            ToRelativePoint(result.Image, BallResultFilter.RedBallPoint)));

    }

    public Point2f? ToRelativePoint(Mat? frame, Point2f? p)
    {
        if (frame == null
            || !p.HasValue)
        {
            return null;
        }

        if (frame.Height > frame.Width)
        {
            return new Point2f(p.Value.Y / frame.Height, 1 - (p.Value.X / frame.Width));
        }
        return new Point2f(p.Value.X / frame.Width, p.Value.Y / frame.Height);
    }
}