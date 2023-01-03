using OpenCvSharp;
using OpenCvSharp.Internal.Vectors;

namespace Billiards.Base.Filters;

public class WarpPerspectiveFilter : AbstractFilter
{
    private IPointsFilter? pointsFilter;
    public IPointsFilter? PointsFilter
    {
        get { return pointsFilter; }
        set { SetProperty(ref pointsFilter, value); }
    }

    public WarpPerspectiveFilter(AbstractFilter filter) : base(filter)
    {
        Name = "Warp perspective";
    }

    protected override void ApplyFilter(Mat originalImage)
    {
        if (PointsFilter?.Points == null
            || PointsFilter.Points.Count < 4)
        {
            return;
        }
        Mat input = GetInputMat();
        ResultMat = input.Clone();
        Point2f[] pointArray = new Point2f[PointsFilter.Points.Count];
        for (int i = 0; i < PointsFilter.Points.Count; i++)
        {
            pointArray[i] = PointsFilter.Points[i];
        }
        VectorOfPoint2f src = new VectorOfPoint2f(pointArray);
        VectorOfPoint2f dest = new VectorOfPoint2f(new[]
        {
                new Point2f(0, 0),
                new Point2f(input.Width, 0),
                new Point2f(input.Width, input.Height),
                new Point2f(0, input.Height)
        });
        Mat warpingMat = Cv2.GetPerspectiveTransform(src.ToArray(), dest.ToArray());
        Cv2.WarpPerspective(input, ResultMat, warpingMat, input.Size());
    }
}