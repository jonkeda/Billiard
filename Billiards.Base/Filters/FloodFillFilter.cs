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

    private Mat? mask = new ();
    public Mat? Mask
    {
        get { return mask; }
        set
        {
            SetProperty(ref mask, value);
        }
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
        using Mat newMask = Mat.Zeros(input.Rows + 2, input.Cols + 2, MatType.CV_8U, 1);

        float floodFillDiff = 1.5f;
        Point mid;
        if (Point2filter?.Point2f != null)
        {
            mid = new Point((int)Point2filter.Point2f.X, (int)Point2filter.Point2f.Y);
        }
        else
        {
            mid = new Point(input.Cols / 2, input.Rows / 2 + yStep);
        }
        Cv2.FloodFill(ResultMat, newMask, mid, new Scalar(FloodFillColor), 
            out boundingRect, floodFillDiff, floodFillDiff, (FloodFillFlags)(4 | (255 << 8)) );
        Mask = newMask.SubMat(1, newMask.Rows - 2, 1, newMask.Cols - 2);

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