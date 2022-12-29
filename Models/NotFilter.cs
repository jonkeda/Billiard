using Emgu.CV;

namespace Billiard.Models;

public class NotFilter : AbstractFilter
{
    private IMaskFilter maskFilter;
    public IMaskFilter MaskFilter
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

        CvInvoke.BitwiseNot(input, ResultMat);
    }
}