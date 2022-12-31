using System.Collections.Generic;
using Billiard.Camera.vision.Geometries;
using Emgu.CV;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Point = System.Windows.Point;

namespace Billiard.Models;

public class FindCornerGoodFeaturesFilter : AbstractFilter, IPointsFilter
{
    private List<Point> points;

    public List<Point> Points
    {
        get { return points; }
        set { SetProperty(ref points, value); }
    }

    public FindCornerGoodFeaturesFilter(AbstractFilter filter) : base(filter)
    {
        Name = "Find corner good features";
    }


    protected override void ApplyFilter(Mat originalImage)
    {
        // create corner strength image and do Harris
        var corners = new Mat();
        ResultMat = GetInputMat();
        using GFTTDetector detector = new GFTTDetector(20, 0.5, 20, 3, false, 0.04);

        MKeyPoint[] mPoints = detector.Detect(ResultMat);


        Draw(dc =>
        {
            System.Windows.Media.Brush brush = System.Windows.Media.Brushes.GreenYellow;
            foreach (MKeyPoint mPoint in mPoints)
            {
                    dc.DrawEllipse(brush, null, mPoint.Point.AsPoint(), 3, 3);
            }
        });

    }

}

