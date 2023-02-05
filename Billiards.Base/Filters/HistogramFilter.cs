using Billiards.Base.Drawings;
using OpenCvSharp;

namespace Billiards.Base.Filters;

public class HistogramFilter : AbstractFilter, ISingleContourFilter
{
    public HistogramFilter(AbstractFilter filter) : base(filter)
    {
        Name = "Histogram";
        ImageStretch = ImageStretch.Fill;
    }

    private IMaskFilter? maskFilter;
    public IMaskFilter? MaskFilter
    {
        get { return maskFilter; }
        set { SetProperty(ref maskFilter, value); }
    }

    public int Start { get; set; } = 0;
    public int End { get; set; } = 256;

    private IContourFilter? contourFilter;
    public IContourFilter? ContourFilter
    {
        get { return contourFilter; }
        set { SetProperty(ref contourFilter, value); }
    }
    public int MaskContour { get; set; }

    public bool Channel0 { get; set; } = true;
    public bool Channel1 { get; set; } = false;
    public bool Channel2 { get; set; } = false;

    public bool HsvShift { get; set; } = false;
    public int HsvShiftBy { get; set; }

    private Rect MatchRect(Mat mat, Rect bounds)
    {
        int left = Math.Max(0, bounds.Left);
        int right = Math.Min(mat.Width, bounds.Right);

        int top = Math.Max(0, bounds.Top);
        int bottom = Math.Min(mat.Height, bounds.Bottom);

        int width = right - left;
        int height = bottom - top;

        return new Rect(left, top, width, height);
    }

    protected override void ApplyFilter(Mat originalImage)
    {
        ResultMat = GetInputMat();

        if (contourFilter?.Contours != null
            && contourFilter.Contours.Count > MaskContour
            && contourFilter.Contours[MaskContour].RotatedRectangle.HasValue)
        {
            ResultMat = ResultMat.Clone(MatchRect(ResultMat,contourFilter.Contours[MaskContour].RotatedRectangle.Value.BoundingRect()));
        }


        Mat? mask = null;
        if (MaskFilter?.Mask != null)
        {
            mask = MaskFilter.Mask;
        }

        Mat[] mats = new Mat[1];
        mats[0] = GetInputMat();

        List<Mat> hists = new List<Mat>();
        for (int i = 0; i < ResultMat.Channels(); i++)
        {
            if ((i == 0 && Channel0)
                || (i == 1 && Channel1)
                || (i == 2 && Channel2))
            {
                Mat hist = new Mat();
                Cv2.CalcHist(mats, new[] {i}, mask, hist, 1, new[] {256}, new[] {new Rangef(Start, End)});
                hists.Add(hist);
            }
        }

        if (DrawImage)
        {
            DrawingImage = DrawHist(hists);
            //ResultMat = new Mat();
        }

        for (int i = 0; i < hists.Count; i++)
        {
            float mean = CalculateMean(hists[i]);
            if (mean == float.NaN)
            {
                string a = "?";
            }

            FilterValues.Add($"{i} Avg Max", CalculateAverageMax(hists[i]));

            FilterValues.Add($"{i} Mean", System.Math.Round(mean, 0));
            FilterValues.Add($"{i} Max", CalculateMax(hists[i]));
        }

    }

    private float CalculateMean(Mat hist)
    {
        float total = 0;
        float totalSummed = 0;
        //float[,] data = (float[,])hist.GetData();
        for (int i = Start; i < hist.Size().Height && i < End; i++)
        {
            float value = hist.Get<float>(i, 0);
            total += value;
            totalSummed += i * value;
        }

        return totalSummed / total;
    }

    private int CalculateAverageMax(Mat hist)
    {
        int index = 0;
        float max = 0;
        // float[,] data = (float[,])hist.GetData();

        float c1 = 0;
        float c2 = 0;
        float c3 = 0;

        for (int i = Start; i < hist.Size().Height && i < End; i++)
        {
            c3 = hist.Get<float>(i, 0);
            float total = c1 + c2 + c3;
            // float value = data[i, 0];
            if (total > max)
            {
                max = total;
                index = IndexShift(i);
            }

            c1 = c2;
            c2 = c3;
        }

        return index;
    }

    private int IndexShift(int index)
    {
        if (HsvShift)
        {
            if (index > (180 - HsvShiftBy))
            {
                return index - 180;
            }

            return index + HsvShiftBy;
        }

        return index;
    }


    private int CalculateMax(Mat hist)
    {
        int index = 0;
        float max = 0;
        // float[,] data = (float[,])hist.GetData();
        for (int i = Start; i < hist.Size().Height && i < End; i++)
        {
            float value = hist.Get<float>(i, 0);

            // float value = data[i, 0];
            if (value > max)
            {
                max = value;
                index = IndexShift(i);
            }
        }

        return index;
    }

    private DrawingImage DrawHist(List<Mat> hists)
    {
        float maximum = 0;

        foreach (Mat hist in hists)
        {
            //hist.SetFloatValue(0, 0, 0);
            //hist.SetFloatValue(0, 255, 0);

            double minval;
            double maxval;
            Point minp;
            Point maxp;

            Cv2.MinMaxLoc(hist, out minval, out maxval, out minp, out maxp);
            maximum = System.MathF.Max((float)maxval, maximum);
        }

        float histWidth = 256 * hists.Count;

        DrawingVisual visual = new DrawingVisual();
        using (DrawingContext drawingContext = visual.RenderOpen())
        {
            drawingContext.PushClip(new RectangleGeometry(
                new Rect2f(new Point2f(0, 0),
                    new Size2f(histWidth, maximum))));
            drawingContext.DrawRectangle(Brushes.Transparent, null,
                new Rect2f(0, 0, histWidth, maximum));

            int width = 1;
            Pen color = null;

            for (int h = 0; h < hists.Count; h++)
            {
                if (h == 0)
                {
                    color = new Pen(Brushes.Blue, width);
                }
                else if (h == 1)
                {
                    color = new Pen(Brushes.Green, width);
                }
                else if (h == 2)
                {
                    color = new Pen(Brushes.Red, width);
                }
                else
                {
                    color = new Pen(Brushes.Black, width);
                }
                var hist = hists[h];
                //float[,] data = (float[,])hist.GetData();
                //for (int i = Start; i < hist.Size().Height && i < End; i++)
                for (int i = 0; i < hist.Size().Height; i++)
                {
                    drawingContext.DrawLine(color,
                    new Point2f(i * width + h * 256, maximum),
                    new Point2f(i * width + h * 256, maximum - hist.Get<float>(i, 0)));
                }

            }
            drawingContext.Close();
        }

        return new DrawingImage(visual.Drawing);
    }

}