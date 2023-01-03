using Billiards.Base.Filters;

namespace Billiards.Base.FilterSets;

public class BallDetectorSet : FilterSet
{
    public BallResultFilter BallResultFilter { get; private set; }

    public BallDetectorSet(AbstractFilter? filter) : base("Find balls")
    {
        var original = Clone(filter);
        var hsv = CvtColorBgr2Hsv();
        ExtractChannel(hsv, 0);
        var gaus = GaussianBlur();
        // Filters.AddFilter(new FindPointByColorFilter(gaus));
        var flood = FloodFill(255);
        flood.MinimumArea = 20;
        Mask();
        MorphClose();
        var morph = MorphOpen();
        FloodFillCorners(255);

        Not();
        var masked = ToMask();
        var canny = Canny();
        var contours = Contours();
        contours.MinimumArea = 1000;
        contours.MaximumArea = 5000;
        contours.Resize = 0.7;

        // And(hsv).MaskFilter = masked;
        // Histogram().MaskFilter = masked;

        // And(original).MaskFilter = masked;
        // Histogram().MaskFilter = masked;

        var and1 = And(hsv);
        and1.ContourFilter = contours;
        and1.MaskContour = 0;
        var hist0 = Histogram();
        hist0.Start = 1;
        hist0.End = 100;

        var and2 = And(hsv);
        and2.ContourFilter = contours;
        and2.MaskContour = 1;
        var hist1 = Histogram();
        hist1.Start = 1;
        hist1.End = 100;

        var and3 = And(hsv);
        and3.ContourFilter = contours;
        and3.MaskContour = 2;
        var hist2 = Histogram();
        hist2.Start = 1;
        hist2.End = 100;

        var result = Filters.AddFilter(new BallResultFilter(original));
        result.ContourFilter = contours;
        result.Histogram0 = hist0;
        result.Histogram1 = hist1;
        result.Histogram2 = hist2;

        BallResultFilter = result;
    }
}