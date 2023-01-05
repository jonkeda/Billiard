using Billiard.Models;

namespace Billiard.viewModels;

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