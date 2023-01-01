using Emgu.CV;
using Emgu.CV.CvEnum;

namespace Billiard.Models;

public class CvtColorBgr2GrayFilter : AbstractFilter
{
    public CvtColorBgr2GrayFilter(AbstractFilter filter) : base(filter)
    {
        Name = "BGR to GRAY";
    }

    protected override void ApplyFilter(Mat originalImage)
    {
        CvInvoke.CvtColor(GetInputMat(), ResultMat, ColorConversion.Bgr2Gray);
    }
}