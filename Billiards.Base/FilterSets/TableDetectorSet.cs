using Billiards.Base.Filters;
using OpenCvSharp;

namespace Billiards.Base.FilterSets;

public class TableDetectorSet : FilterSet
{
    public OriginalFilter OriginalFilter { get; private set; }
    public AbstractFilter FoundFilter { get; private set; }

    public TableDetectorSet() : base("Find table")
    {
        var original = Original();
        var hsv = CvtColorBgr2Hsv();
        ExtractChannel(hsv, 0);
        GaussianBlur();

        MorphClose();
        // MorphOpen();
        var flood = FloodFill(255);
        flood.AllowZero = false;
        flood.MinimumArea = 14;
        flood.MaximumArea = 80;
        flood.FloodFillDiff = 10;
        flood.FloodFillFlags = FloodFillFlags.FixedRange;

        Mask();

        OriginalFilter = original;
        FoundFilter = flood;
    }

    public AbstractFilter? ResultFilter()
    {
        return Filters.LastOrDefault();
    }
}