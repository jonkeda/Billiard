using Emgu.CV;

namespace Billiard.Models;

public class ToMaskFilter : AbstractFilter, IMaskFilter
{
    private Mat mask;

    public ToMaskFilter(AbstractFilter filter) : base(filter)
    {
        Name = "To Mask";
    }

    protected override void ApplyFilter(Mat originalImage)
    {
        ResultMat = GetInputMat();
        Mask = GetInputMat();
    }

    public Mat Mask
    {
        get { return mask; }
        set { SetProperty(ref mask, value); }
    }
}