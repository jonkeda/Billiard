using Emgu.CV;

namespace Billiard.Models;

public class MaskFilter : AbstractFilter
{
    public MaskFilter(AbstractFilter filter) : base(filter)
    {
        Name = "Mask";
    }

    protected override void ApplyFilter(Mat originalImage)
    {
        if (InputFilter is IMaskFilter maskFilter)
        {
            ResultMat = maskFilter.Mask;
        }
    }
}

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