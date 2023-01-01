using OpenCvSharp;

namespace Billiards.Base.Filters;

public class FloodFillFilter : AbstractFilter, IBoundingRectFilter, IMaskFilter
{
    private Rect boundingRect;
    public int FloodFillColor { get; }

    public Rect BoundingRect
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

    public IPointFilter Point2filter { get; set; }
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
        mask = Mat.Zeros(input.Rows + 2, input.Cols + 2, MatType.CV_8U, 1);

        Scalar newColor = new Scalar(FloodFillColor);
        float floodFillDiff = 1.5f;
        Scalar diff = new Scalar(floodFillDiff, floodFillDiff, floodFillDiff);

        Point2f mid;
        if (Point2filter?.Point2f != null)
        {
            mid = new Point2f((int)Point2filter.Point2f.X, (int)Point2filter.Point2f.Y);
        }
        else
        {
            mid = new Point2f(input.Cols / 2, input.Rows / 2 + yStep);
        }

/*
        Cv2.FloodFill(ResultMat, mask, 
            mid, 
            newColor,
            out boundingRect, diff, diff, Connectivity.FourConnected,
            (FloodFillType) (4 | (255 << 8)));
*/
        Cv2.FloodFill(ResultMat, null, new Point(ResultMat.Cols - 1, 0), new Scalar(255), 
            out boundingRect, floodFillDiff, floodFillDiff);


        Rect roi = new Rect(1, 1, mask.Cols - 2, mask.Rows - 2);
        Mat srcROI = new Mat(mask, roi);

        //Mat dstROI = new Mat(dst, roi);

        srcROI.CopyTo(mask);
        // Cv2.BitwiseNot(mask, mask);

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