    using Emgu.CV;

namespace Billiard.Models;

public class CannyFilter : AbstractFilter
{
    public CannyFilter(AbstractFilter filter) : base(filter)
    {
        Name = "Canny";
    }

    protected override void ApplyFilter(Mat originalImage)
    {

        float cannyThreshold = 180.0f;
        float cannyThresholdLinking = 120.0f;
        CvInvoke.Canny(GetInputMat(), ResultMat, cannyThreshold, cannyThresholdLinking);
    }
}