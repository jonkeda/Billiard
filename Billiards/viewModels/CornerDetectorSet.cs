using System.Linq;
using Billiard.Models;
using Emgu.CV.CvEnum;

namespace Billiard.viewModels;

public class CornerDetectorSet : FilterSet
{
    public IPointsFilter PointsFilter { get; private set; }

    public CornerDetectorSet(OriginalFilter original, FloodFillFilter flood ) : base("Find corner")
    {
        var asMask = Mask(flood);
        // DrawBoundingRect().BoundingRect = flood;
        //var corners = FindCorners();
        //corners.BoundingRect = flood;
        //Filters.AddFilter(new FindCornerHarrisFilter(asMask));
        //FloodFillCorners(255);

        //var canny = Canny(asMask);
        // Filters.AddFilter(new FindCornerGoodFeaturesFilter(canny));

        var contour = Contours();
        contour.ContourType = ContourType.ConvexHull;
        contour.ChainApproxMethod = ChainApproxMethod.ChainApproxSimple;
        contour.MinimumArea = 0;
             
        var convexConvers = Filters.AddFilter(new FindCornersConvexHullFilter(contour));
        convexConvers.ContourFilter = contour;

        PointsFilter = convexConvers;

        WarpPerspective(original).PointsFilter = convexConvers;
    }

    public AbstractFilter ResultFilter()
    {
        return Filters.LastOrDefault();
    }
}