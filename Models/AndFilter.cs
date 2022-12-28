using Emgu.CV;

namespace Billiard.Models;

public class AndFilter : AbstractFilter
{
    public AndFilter(AbstractFilter filter) : base(filter)
    {
        Name = "And";
    }

    private IMaskFilter maskFilter;
    public IMaskFilter MaskFilter
    {
        get { return maskFilter; }
        set { SetProperty(ref maskFilter, value); }
    }


    protected override void ApplyFilter(Mat originalImage)
    {
        Mat input = GetInputMat();
        ResultMat = new Mat();
/*        double minval = 0;
        double maxval = 0;
        System.Drawing.Point minp = Point.Empty;
        System.Drawing.Point maxp = Point.Empty;

        CvInvoke.MinMaxLoc(maskFilter.Mask, ref minval, ref maxval, ref minp, ref maxp);
*/
        CvInvoke.BitwiseAnd(input, input, ResultMat, MaskFilter.Mask);
    }
}

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