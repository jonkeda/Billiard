using OpenCvSharp;

namespace Billiards.Base.Filters;

public class GaussianBlurFilter : AbstractFilter
{
    public GaussianBlurFilter(AbstractFilter filter) : base(filter)
    {
        Name = "Gausian";
    }

    protected override void ApplyFilter(Mat originalImage)
    {

        Cv2.GaussianBlur(GetInputMat(), ResultMat, new Size(3, 3), 1);
    }
}