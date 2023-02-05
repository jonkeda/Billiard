using Billiards.Base.Filters;
using OpenCvSharp;

namespace Billiards.Base.FilterSets;

public class CornerDetectorSet : FilterSet
{
    public IPointsFilter PointsFilter { get; private set; }

    public CornerDetectorSet(OriginalFilter original, AbstractFilter flood ) : base("Find corner")
    {
        Mask(flood);

        /*        Resize(0.5f);
                Resize(2f);
        */

        float size = 10;
        var mc = MorphClose();
        mc.MorphShapes = MorphShapes.Rect;
        mc.Size = new Size(size, size);
/*        var mo = MorphOpen();
        mo.MorphShapes = MorphShapes.Rect;
        mo.Size = new Size(size, size);
*/

        var contour = Contours();
        contour.ContourType = ContourType.Approximated;
        contour.ChainApproxMethod = ContourApproximationModes.ApproxNone;
        contour.MinimumArea = 0;
        contour.ApproximateEps = 5;


        var convexConvers = Filters.AddFilter(new FindCornersConvexHullFilter(contour));
        convexConvers.ContourFilter = contour;
        convexConvers.StraigthenAngle = 5;

        PointsFilter = convexConvers;

        WarpPerspective(original).PointsFilter = convexConvers;
    }

    public AbstractFilter? ResultFilter()
    {
        return Filters.LastOrDefault();
    }
}