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

    public IPointFilter PointFilter { get; set; }
    public int MinimumArea { get; set; }
    public int MaximumArea { get; set; } = 100;

    public FloodFillFilter(AbstractFilter filter, int floodFillColor) : base(filter)
    {
        Name = "FloodFill";
        FloodFillColor = floodFillColor;
    }

    protected override void ApplyFilter(Mat originalImage)
    {
        if (FindArea(originalImage, 0))
        {
            return;
        }
        if (FindArea(originalImage, 50))
        {
            return;
        }
        if (FindArea(originalImage, -50))
        {
            return;
        }
        if (FindArea(originalImage, 100))
        {
            return;
        }
        if (FindArea(originalImage, -100))
        {
            return;
        }
    }

    protected bool FindArea(Mat originalImage, int yStep)
    {
        Mat input = GetInputMat();
        ResultMat = input.Clone();
        mask = Mat.Zeros(input.Rows + 2, input.Cols + 2, DepthType.Cv8U, 1);

        MCvScalar newColor = new MCvScalar(FloodFillColor);
        float floodFillDiff = 1.5f;
        MCvScalar diff = new MCvScalar(floodFillDiff, floodFillDiff, floodFillDiff);

        Point mid;
        if (PointFilter?.Point != null)
        {
            mid = new Point((int)PointFilter.Point.X, (int)PointFilter.Point.Y);
        }
        else
        {
            mid = new Point(input.Cols / 2, input.Rows / 2 + yStep);
        }

        CvInvoke.FloodFill(ResultMat, mask, 
            mid, 
            newColor,
            out boundingRect, diff, diff, Connectivity.FourConnected,
            (FloodFillType) (4 | (255 << 8)));


        Rectangle roi = new Rectangle(1, 1, mask.Cols - 2, mask.Rows - 2);
        Mat srcROI = new Mat(mask, roi);

        //Mat dstROI = new Mat(dst, roi);

        srcROI.CopyTo(mask);
        // CvInvoke.BitwiseNot(mask, mask);

        double area = boundingRect.Width * boundingRect.Height;
        double fullArea = input.Cols * input.Rows;
        double areaPerc = System.Math.Round((area / fullArea) * 100d);
        FilterValues.Add("Area", area);
        FilterValues.Add("Full Area", fullArea);
        FilterValues.Add("Area %",  areaPerc);
        FilterValues.Add("Mid", mid.ToString());
        FilterValues.Add("Bounds", boundingRect.ToString());

        return  areaPerc >= MinimumArea && areaPerc <= MaximumArea;
    }
}