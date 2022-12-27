using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace Billiard.Models;

public class FloodFillFilter : AbstractFilter, IBoundingRectFilter, IMaskFilter
{
    private Rectangle boundingRect;
    public int FloodFillColor { get; }

    public Rectangle BoundingRect
    {
        get { return boundingRect; }
        set { SetProperty(ref boundingRect, value); }
    }

    private Mat mask = new ();
    public Mat Mask
    {
        get { return mask; }
        set { SetProperty(ref mask, value); }
    }

    public FloodFillFilter(AbstractFilter filter, int floodFillColor) : base(filter)
    {
        Name = "FloodFill";
        FloodFillColor = floodFillColor;
    }

    protected override void ApplyFilter(Mat originalImage)
    {
        Mat input = GetInputMat();
        ResultMat = input.Clone();
        mask = Mat.Zeros(input.Rows + 2, input.Cols + 2, DepthType.Cv8U, 1);

        MCvScalar newColor = new MCvScalar(FloodFillColor);
        float floodFillDiff = 1.5f;
        MCvScalar diff = new MCvScalar(floodFillDiff, floodFillDiff, floodFillDiff);

        CvInvoke.FloodFill(ResultMat, mask, 
            new Point(input.Cols / 2, input.Rows / 2), 
            newColor,
            out boundingRect, diff, diff, Connectivity.FourConnected,
            (FloodFillType) (4 | (255 << 8)));

        // CvInvoke.BitwiseNot(mask, mask);
    }
}