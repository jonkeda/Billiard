using OpenCvSharp;
using OpenCvSharp.Internal.Vectors;

namespace Billiards.Base.Filters;

public class HistogramFilter : AbstractFilter
{
    public HistogramFilter(AbstractFilter filter) : base(filter)
    {
        Name = "Histogram";
    }

    private IMaskFilter maskFilter;
    public IMaskFilter MaskFilter
    {
        get { return maskFilter; }
        set { SetProperty(ref maskFilter, value); }
    }

    public int Start { get; set; } = 0;
    public int End { get; set; } = 256;

    protected override void ApplyFilter(Mat originalImage)
    {
        ResultMat = GetInputMat();

/*        VectorOfMat vou = new VectorOfMat();
        vou.Push(ResultMat);
*/
        Mat? mask = null;
        if (maskFilter?.Mask != null)
        {
            mask = maskFilter.Mask;
        }

        Mat[] mats = new Mat[1];
        mats[0] = originalImage;

        List<Mat> hists = new List<Mat>();
        for (int i = 0; i < ResultMat.Channels(); i++)
        {
            Mat hist = new Mat();
            Cv2.CalcHist(mats, new [] {i}, mask, hist, 1, new [] {256}, new [] {new Rangef(0, 256)});
            hists.Add(hist);
        }

        // todo
        // DrawingImage = DrawHist(hists);

        ResultMat = new Mat();

        for (int i = 0; i < hists.Count; i++)
        {
            FilterValues.Add($"{i} Mean", System.Math.Round(CalculateMean(hists[i]), 0));
            FilterValues.Add($"{i} Max", CalculateMax(hists[i]));
        }

        /*        float[,] data = (float[,])hists[0].GetData();
                for (int i = 0; i < data.Length; i++)
                {
                    FilterValues.Add(i.ToString(), ((int)data[i, 0]).ToString());
                }*/

    }

    private float CalculateMean(Mat hist)
    {
        float total = 0;
        float totalSummed = 0;
        //float[,] data = (float[,])hist.GetData();
        for (int i = Start; i < hist.Size().Width && i < End; i++)
        {
            float value = hist.Get<float>(i, 0);
            total += value;
            totalSummed += i * value;
        }

        return totalSummed / total;
    }

    private int CalculateMax(Mat hist)
    {
        int index = 0;
        float max = 0;
        // float[,] data = (float[,])hist.GetData();
        for (int i = Start; i < hist.Size().Width && i < End; i++)
        {
            float value = hist.Get<float>(i, 0);

            // float value = data[i, 0];
            if (value > max)
            {
                max = value;
                index = i;
            }
        }

        return index;
    }
/*
    private DrawingImage DrawHist(List<Mat> hists)
    {
        double maximum = 0;

        foreach (Mat hist in hists)
        {
            hist.SetFloatValue(0, 0, 0);
            hist.SetFloatValue(0, 255, 0);

            double minval = 0;
            double maxval = 0;
            Point2f minp = Point2f.Empty;
            Point2f maxp = Point2f.Empty;

            Cv2.MinMaxLoc(hist, ref minval, ref maxval, ref minp, ref maxp);
            maximum = System.Math.Max(maxval, maximum);
        }

        double histWidth = 256 * hists.Count;

        DrawingVisual visual = new DrawingVisual();
        using (DrawingContext drawingContext = visual.RenderOpen())
        {
            drawingContext.PushClip(new RectangleGeometry(
                new Rect(new Point2f(0, 0),
                    new Point2f(histWidth, maximum))));
            drawingContext.DrawRectangle(new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 0, 0, 0)), null,
                new Rect(0, 0, histWidth, maximum));

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
                float[,] data = (float[,])hist.GetData();
                for (int i = 0; i < data.Length; i++)
                {
                    drawingContext.DrawLine(color, new Point2f(i * width + h * 256, maximum), new Point2f(i * width + h * 256, maximum -(int)data[i, 0]));
                }

            }
            drawingContext.Close();
        }

        return new DrawingImage(visual.Drawing);
    }*/

}