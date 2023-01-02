using Billiards.Base.Filters;
using OpenCvSharp;

namespace Billiards.Base.FilterSets
{
    public class ResultModel
    {
        public Mat Image { get; set; }

        public List<Point2f> Corners { get; set; }
        public Rect2f TableSize { get; set; }
        public Point2f? WhiteBallPoint { get; set; }
        public Point2f? YellowBallPoint { get; set; }
        public Point2f? RedBallPoint { get; set; }
        public CaramboleDetector Detector { get; set; }
        public DateTime Now { get; set; }
    }


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
}
