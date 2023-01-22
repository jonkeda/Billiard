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


        var contour = Contours();
        contour.ContourType = ContourType.ConvexHull;
        contour.ChainApproxMethod = ContourApproximationModes.ApproxSimple;
        contour.MinimumArea = 0;
             
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