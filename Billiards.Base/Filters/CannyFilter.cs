    using OpenCvSharp;

    namespace Billiards.Base.Filters;

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
        Cv2.Canny(GetInputMat()!, ResultMat!, cannyThreshold, cannyThresholdLinking);
    }
}