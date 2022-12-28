using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace Billiard.Models;

public class FloodFillCornersFilter : AbstractFilter
{
    public int FloodFillColor { get; }

    public FloodFillCornersFilter(AbstractFilter filter, int floodFillColor) : base(filter)
    {
        Name = "FloodFill Corners";
        FloodFillColor = floodFillColor;
    }

    protected override void ApplyFilter(Mat originalImage)
    {
        Mat input = GetInputMat();
        ResultMat = input.Clone();

        MCvScalar newColor = new MCvScalar(FloodFillColor);
        float floodFillDiff = 0;
        MCvScalar diff = new MCvScalar(floodFillDiff, floodFillDiff, floodFillDiff);

        CvInvoke.FloodFill(ResultMat, null, 
            new Point(0, 0), 
            newColor,
            out _, diff, diff, Connectivity.FourConnected,
            (FloodFillType) (4 | (255 << 8)));

        CvInvoke.FloodFill(ResultMat, null,
            new Point(0, ResultMat.Rows -1),
            newColor,
            out _, diff, diff, Connectivity.FourConnected,
            (FloodFillType)(4 | (255 << 8)));

        CvInvoke.FloodFill(ResultMat, null,
            new Point(ResultMat.Cols -1, ResultMat.Rows - 1),
            newColor,
            out _, diff, diff, Connectivity.FourConnected,
            (FloodFillType)(4 | (255 << 8)));

        CvInvoke.FloodFill(ResultMat, null,
            new Point(ResultMat.Cols - 1, 0),
            newColor,
            out _, diff, diff, Connectivity.FourConnected,
            (FloodFillType)(4 | (255 << 8)));
    }
}