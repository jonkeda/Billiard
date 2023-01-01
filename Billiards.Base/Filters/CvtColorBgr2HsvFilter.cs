using OpenCvSharp;

namespace Billiards.Base.Filters;

public class CvtColorBgr2HsvFilter : AbstractFilter
{
    public CvtColorBgr2HsvFilter(AbstractFilter filter) : base(filter)
    {
        Name = "BGR to HSV";
    }

    protected override void ApplyFilter(Mat originalImage)
    {

        Cv2.CvtColor(GetInputMat(), ResultMat, ColorConversionCodes.BGR2HSV);
    }
}