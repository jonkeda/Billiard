using Billiards.Base.Filters;
using OpenCvSharp;

namespace Billiards.Base.FilterSets
{
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

        public (List<Point2f> corners, Rect2d tableSize, 
            Point2f? whiteBallPoint, Point2f? yellowBallPoint, Point2f? redBallPoint) ApplyFilters(Mat image)
        {
            FilterSets.ApplyFilters(image);

            return (PointsFilter.Points,
                BallResultFilter.TableSize,
                BallResultFilter.WhiteBallPoint, 
                BallResultFilter.YellowBallPoint, 
                BallResultFilter.RedBallPoint);
        }
    }
}
