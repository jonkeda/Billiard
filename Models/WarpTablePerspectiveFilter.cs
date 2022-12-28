using System.Drawing;
using Billiard.Camera.vision.Geometries;
using Emgu.CV;
using Emgu.CV.Util;

namespace Billiard.Models;

public class WarpPerspectiveFilter : AbstractFilter
{
    private IPointsFilter pointsFilter;
    public IPointsFilter PointsFilter
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
        if (PointsFilter == null
            || PointsFilter.Points.Count < 4)
        {
            return;
        }
        Mat input = GetInputMat();
        ResultMat = input.Clone();
        PointF[] pointArray = new PointF[PointsFilter.Points.Count];
        for (int i = 0; i < PointsFilter.Points.Count; i++)
        {
            pointArray[i] = PointsFilter.Points[i].AsPointF();
        }
        VectorOfPointF src = new VectorOfPointF(pointArray);
        VectorOfPointF dest = new VectorOfPointF(new[]
        {
                new PointF(0, 0),
                new PointF(input.Width, 0),
                new PointF(input.Width, input.Height),
                new PointF(0, input.Height)
        });
        Mat warpingMat = CvInvoke.GetPerspectiveTransform(src, dest);
        CvInvoke.WarpPerspective(input, ResultMat, warpingMat, input.Size);
    }
}