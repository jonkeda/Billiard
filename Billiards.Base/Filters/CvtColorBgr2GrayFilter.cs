using OpenCvSharp;

namespace Billiards.Base.Filters;

public class CvtColorBgr2GrayFilter : AbstractFilter
{
    public CvtColorBgr2GrayFilter(AbstractFilter filter) : base(filter)
    {
        Name = "BGR to GRAY";
    }

    protected override void ApplyFilter(Mat originalImage)
    {
        Cv2.CvtColor(GetInputMat(), ResultMat, ColorConversionCodes.BGR2GRAY);
    }
}