using OpenCvSharp;

namespace Billiards.Base.Filters;

public class NotFilter : AbstractFilter
{
    private IMaskFilter? maskFilter;
    public IMaskFilter? MaskFilter
    {
        get { return maskFilter; }
        set { SetProperty(ref maskFilter, value); }
    }

    public NotFilter(AbstractFilter filter) : base(filter)
    {
        Name = "Not";
    }

    protected override void ApplyFilter(Mat originalImage)
    {
        Mat input = GetInputMat();

        Cv2.BitwiseNot(input, ResultMat);
    }
}