using Emgu.CV;
using Emgu.CV.CvEnum;

namespace Billiard.Models;

public class CvtColorBgr2HsvFilter : AbstractFilter
{
    public CvtColorBgr2HsvFilter(AbstractFilter filter) : base(filter)
    {
        Name = "BGR to HSV";
    }

    protected override void ApplyFilter(Mat originalImage)
    {

        CvInvoke.CvtColor(GetInputMat(), ResultMat, ColorConversion.Bgr2Hsv);
    }
}