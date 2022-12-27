using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace Billiard.Models;

public class GaussianBlurFilter : AbstractFilter
{
    public GaussianBlurFilter(AbstractFilter filter) : base(filter)
    {
        Name = "Gausian";
    }

    protected override void ApplyFilter(Mat originalImage)
    {

        CvInvoke.GaussianBlur(GetInputMat(), ResultMat, new Size(3, 3), 1);
    }
}

public class MorphOpenFilter : AbstractFilter
{
    public MorphOpenFilter(AbstractFilter filter) : base(filter)
    {
        Name = "Morph Open";
    }

    protected override void ApplyFilter(Mat originalImage)
    {
        Mat kernelOp = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(3, 3), new Point(-1, -1));
        CvInvoke.MorphologyEx(GetInputMat(), ResultMat, MorphOp.Open, kernelOp, new Point(-1, -1), 1, BorderType.Default, new MCvScalar());
    }
}

public class MorphCloseFilter : AbstractFilter
{
    public MorphCloseFilter(AbstractFilter filter) : base(filter)
    {
        Name = "Morph Close";
    }

    protected override void ApplyFilter(Mat originalImage)
    {
        Mat kernelOp = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(3, 3), new Point(-1, -1));
        CvInvoke.MorphologyEx(GetInputMat(), ResultMat, MorphOp.Close, kernelOp, new Point(-1, -1), 1, BorderType.Default, new MCvScalar());
    }
}
