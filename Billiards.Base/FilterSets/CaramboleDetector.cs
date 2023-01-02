using Billiards.Base.Filters;

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

    public BallResultFilter BallResultFilter { get; private set; }
    public IPointsFilter PointsFilter { get; set; }
    public FilterSetCollection FilterSets { get; } = new();

    public void ApplyFilters(ResultModel result)
    {
        FilterSets.ApplyFilters(result.Image);

        result.Corners = PointsFilter.Points;
        result.WhiteBallPoint = BallResultFilter.WhiteBallPoint;
        result.YellowBallPoint = BallResultFilter.YellowBallPoint;
        result.RedBallPoint = BallResultFilter.RedBallPoint;
    }
}