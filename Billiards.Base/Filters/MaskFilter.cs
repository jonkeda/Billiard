using OpenCvSharp;

namespace Billiards.Base.Filters;

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