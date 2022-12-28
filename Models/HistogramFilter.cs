using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Billiard.Camera.vision.Geometries;
using Emgu.CV;
using Emgu.CV.Util;

namespace Billiard.Models;

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

    protected override void ApplyFilter(Mat originalImage)
    {
        ResultMat = GetInputMat();

        VectorOfMat vou = new VectorOfMat();
        vou.Push(ResultMat);

        Mat mask = null;
        if (maskFilter?.Mask?.GetData() != null)
        {
            mask = maskFilter.Mask;
        }


        List<Mat> hists = new List<Mat>();
        for (int i = 0; i < ResultMat.NumberOfChannels; i++)
        {
            Mat hist = new Mat();
            CvInvoke.CalcHist(vou, new int[] { i }, mask, hist, new int[] { 256 }, new float[] { 0, 256 }, false);
            hists.Add(hist);
        }

        DrawingImage = DrawHist(hists);

        ResultMat = new Mat();
    }


    private DrawingImage DrawHist(List<Mat> hists)
    {
        double maximum = 0;

        foreach (Mat hist in hists)
        {
            hist.SetFloatValue(0, 0, 0);
            hist.SetFloatValue(0, 255, 0);

            double minval = 0;
            double maxval = 0;
            System.Drawing.Point minp = System.Drawing.Point.Empty;
            System.Drawing.Point maxp = System.Drawing.Point.Empty;

            CvInvoke.MinMaxLoc(hist, ref minval, ref maxval, ref minp, ref maxp);
            maximum = System.Math.Max(maxval, maximum);
        }

        double histWidth = 256 * hists.Count;

        DrawingVisual visual = new DrawingVisual();
        using (DrawingContext drawingContext = visual.RenderOpen())
        {
            drawingContext.PushClip(new RectangleGeometry(
                new Rect(new Point(0, 0),
                    new Point(histWidth, maximum))));
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
                    drawingContext.DrawLine(color, new Point(i * width + h * 256, maximum), new Point(i * width + h * 256, maximum -(int)data[i, 0]));
                }

            }
            drawingContext.Close();
        }

        return new DrawingImage(visual.Drawing);
    }

}