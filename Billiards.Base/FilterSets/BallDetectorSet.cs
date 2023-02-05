using Billiards.Base.Filters;
using OpenCvSharp;

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

        flood.FloodFillFlags = FloodFillFlags.Link4;
        flood.FloodFillDiff = 1f;

        //flood.FloodFillFlags = FloodFillFlags.FixedRange;
        //flood.FloodFillDiff = 10;
        flood.MinimumArea = 20;

        Mask();
        var close = MorphClose();
        close.Size = new Size(3, 3);
        var open = MorphOpen();
        open.Size = new Size(3, 3);

        FloodFillCorners(255);

        Not();
        var masked = ToMask();
        var canny = Canny();
        var contours = Contours();
        contours.MinimumArea = 500;
        contours.MaximumArea = 8000;
        contours.MinimumRatio = 0.15d;
        contours.MaximumRatio = 1d;
        contours.Resize = 0.7;


/*        var blob = BlobDetection();
        blob.MinimumArea = 500f;
        blob.MaximumArea = 8000f;
        blob.MinimumRatio = 0.15f;
        blob.MaximumRatio = 1f;
        //contours.Resize = 0.7;*/

        // And(hsv).MaskFilter = masked;
        // Histogram().MaskFilter = masked;

        // And(original).MaskFilter = masked;
        // Histogram().MaskFilter = masked;

        var and0 = And(hsv);
        and0.ContourFilter = contours;
        and0.MaskContour = 0;
        var hist0 = Histogram();
        hist0.Start = 1;
        //hist0.End = 100;
        hist0.ContourFilter = contours;
        hist0.MaskContour = 0;
        hist0.HsvShift = true;
        hist0.HsvShiftBy = 40;

        var and1 = And(hsv);
        and1.ContourFilter = contours;
        and1.MaskContour = 1;
        var hist1 = Histogram();
        hist1.Start = 1;
        //hist1.End = 100;
        hist1.ContourFilter = contours;
        hist1.MaskContour = 1;
        hist1.HsvShift = true;
        hist1.HsvShiftBy = 40;

        var and2 = And(hsv);
        and2.ContourFilter = contours;
        and2.MaskContour = 2;
        var hist2 = Histogram();
        hist2.Start = 1;
        //hist2.End = 100;
        hist2.ContourFilter = contours;
        hist2.MaskContour = 2;
        hist2.HsvShift = true;
        hist2.HsvShiftBy = 40;

        var result = Filters.AddFilter(new BallResultFilter(original));
        result.ContourFilter = contours;
        result.Histogram0 = hist0;
        result.Histogram1 = hist1;
        result.Histogram2 = hist2;

        BallResultFilter = result;
    }
}