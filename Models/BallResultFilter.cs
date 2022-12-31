using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Media;
using Billiard.Camera.vision.Geometries;
using Billiard.Physics;
using Emgu.CV;

namespace Billiard.Models;

public enum BallColor
{
    Red = 0,
    Yellow = 1,
    White  = 2
}

public class BallResult
{
    public Contour Contour { get; set; }
    public double Mean { get; set; }
    public double Max { get; set; }
    public int Index { get; set; }
    public BallColor Color { get; set; }
}

public class BallResultFilter : AbstractFilter
{
    public BallResultFilter(AbstractFilter filter) : base(filter)
    {
        Name = "Ball results";
    }

    private IContourFilter contourFilter;
    public IContourFilter ContourFilter
    {
        get { return contourFilter; }
        set { SetProperty(ref contourFilter, value); }
    }

    private HistogramFilter histogram0;
    public HistogramFilter Histogram0
    {
        get { return histogram0; }
        set { SetProperty(ref histogram0, value); }
    }

    private HistogramFilter histogram1;
    public HistogramFilter Histogram1
    {
        get { return histogram1; }
        set { SetProperty(ref histogram1, value); }
    }

    private HistogramFilter histogram2;
    public HistogramFilter Histogram2
    {
        get { return histogram2; }
        set { SetProperty(ref histogram2, value); }
    }

    public System.Windows.Point? WhiteBallPoint { get; set; }
    public System.Windows.Point? YellowBallPoint { get; set; }
    public System.Windows.Point? RedBallPoint { get; set; }


    protected override void ApplyFilter(Mat originalImage)
    {
        ResultMat = GetInputMat();

        List<BallResult> balls = new ()
        {
            CreateBall(Histogram0, 0),
            CreateBall(Histogram1, 1),
            CreateBall(Histogram2, 2)
        };

        PredictBalls(balls);

        CopyHistogramValues(Histogram0, 0);
        CopyHistogramValues(Histogram1, 1);
        CopyHistogramValues(Histogram2, 2);

        Draw(dc =>
        {
            Pen pen = new Pen(Brushes.Black, 2);
            foreach (BallResult ball in balls)
            {
                FormattedText formattedText = new(
                    ball.Index.ToString(),
                    CultureInfo.CurrentUICulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Verdana"),
                    32,
                    Brushes.AntiqueWhite, 1.25);

                if (ball.Contour?.RotatedRectangle != null)
                {
                    var mid = ball.Contour.RotatedRectangle.Value.Center.AsPoint();
                    dc.DrawText(formattedText, mid);

                    Brush color = null;
                    if (ball.Color == BallColor.Red)
                    {
                        color = Brushes.Red;
                    }
                    else if (ball.Color == BallColor.White)
                    {
                        color = Brushes.White;
                    }
                    else if (ball.Color == BallColor.Yellow)
                    {
                        color = Brushes.Yellow;
                    }

                    dc.DrawEllipse(color, pen, mid, 5, 5);
                }
            }
        });
    }

    private void PredictBalls(List<BallResult> balls)
    {
        RedBallPoint = null;
        WhiteBallPoint = null;
        YellowBallPoint = null;

        int color = 0;
        foreach (BallResult result in balls.OrderBy(b => b.Mean))
        {
            result.Color = (BallColor)color;
            color++;
            if (result?.Contour?.RotatedRectangle != null
                && result.Contour.RotatedRectangle.HasValue)
            {
                var mid = result.Contour.RotatedRectangle.Value.Center.AsPoint();
                if (result.Color == BallColor.Red)
                {
                    RedBallPoint = mid;
                }
                else if (result.Color == BallColor.White)
                {
                    WhiteBallPoint = mid;
                }
                else if (result.Color == BallColor.Yellow)
                {
                    YellowBallPoint = mid;
                }
            }
        }
    }

    private BallResult CreateBall(HistogramFilter histogramFilter, int contourIndex)
    {
        var b = new BallResult();
        b.Index = contourIndex;
        if (ContourFilter.Contours.Count > contourIndex)
        {
            Contour contour = ContourFilter.Contours[contourIndex];
            b.Contour = contour;
        }

        b.Mean = histogramFilter.FilterValues[0].Value;
        b.Max = histogramFilter.FilterValues[1].Value;

        return b;
    }

    private void CopyHistogramValues(HistogramFilter histogramFilter, int number)
    {
        if (histogramFilter != null)
        {
            foreach (FilterValue value in histogramFilter.FilterValues.Take(2))
            {
                FilterValues.Add(new FilterValue($"{number} {value.Name}", value.Value));
            }
        }
    }
}

