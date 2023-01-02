using Billiards.Base.Filters;

namespace Billiards.Base.FilterSets;

public class FindHsvContoursFilterSet : FilterSet
{
    public FindHsvContoursFilterSet() : base("Find HSV contours")
    {
        Original();
        CvtColorBgr2Hsv();
        GaussianBlur();
        Canny();
    }
}