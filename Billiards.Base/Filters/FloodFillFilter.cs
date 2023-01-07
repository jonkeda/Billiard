using Billiards.Base.Drawings;
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

    public IPointFilter? Point2filter { get; set; }
    public int MinimumArea { get; set; }
    public int MaximumArea { get; set; } = 100;
    public float FloodFillDiff { get; set; } = 1.5f;

    public FloodFillFlags FloodFillFlags { get; set; } = FloodFillFlags.Link4;
    public FloodFillFlags? FloodFillFlagsSecondary { get; set; } 

    public FloodFillFilter(AbstractFilter filter, int floodFillColor) : base(filter)
    {
        Name = "FloodFill";
        FloodFillColor = floodFillColor;
    }

    protected override void ApplyFilter(Mat originalImage)
    {
        FilterValues.Add("Minimumarea", MinimumArea);
        FilterValues.Add("MaximumArea", MaximumArea);
        FilterValues.Add("FloodFillFlags", FloodFillFlags.ToString());
        FilterValues.Add("SecondaryFloodFillFlags", FloodFillFlagsSecondary?.ToString());
        FilterValues.Add("FloodFillDiff", FloodFillDiff);

        Mat? input = GetInputMat();
        if (input == null)
        {
            return;
        }

        if (!ApplyFilterArea(input, FloodFillFlags))
        {
            if (FloodFillFlagsSecondary != null)
            {
                ApplyFilterArea(input, FloodFillFlagsSecondary.Value);
            }
        }


        Draw(dc =>
        {
            float radius = Math.Max(input.Cols, input.Rows) / 100f;
            int yStep = input.Rows / 20;

            float x = input.Cols / 2f;
            float y = input.Rows / 2f;

            dc.DrawEllipse(Brushes.GreenYellow, null, new Point2f(x, y), radius, radius);

            dc.DrawEllipse(Brushes.Blue, null, new Point2f(x, y + yStep), radius, radius);

            dc.DrawEllipse(Brushes.LightBlue, null, new Point2f(x, y + 2 * yStep), radius, radius);

            dc.DrawEllipse(Brushes.DarkRed, null, new Point2f(x, y - yStep), radius, radius);

            dc.DrawEllipse(Brushes.Red, null, new Point2f(x, y - 2 * yStep), radius, radius);

        });
    }

    protected bool ApplyFilterArea(Mat input, FloodFillFlags floodFillFlags)
    {
        int yStep = input.Rows / 20;

        if (FindArea(input, yStep, floodFillFlags))
        {
            return true;
        }
        if (FindArea(input, 0, floodFillFlags))
        {
            return true;
        }
        if (FindArea(input, -yStep, floodFillFlags))
        {
            return true;
        }
        if (FindArea(input, 2 * yStep, floodFillFlags))
        {
            return true;
        }
        if (FindArea(input, - 2 * yStep, floodFillFlags))
        {
            return true;
        }

        return false;
    }

    protected bool FindArea(Mat input, int yStep, FloodFillFlags floodFillFlags)
    {
        ResultMat = input.Clone();
        using Mat newMask = Mat.Zeros(input.Rows + 2, input.Cols + 2, MatType.CV_8U, 1);

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
            out boundingRect, FloodFillDiff, FloodFillDiff, 
            (FloodFillFlags)((int)floodFillFlags | (255 << 8)) );
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