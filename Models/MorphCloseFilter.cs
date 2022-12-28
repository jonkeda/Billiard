using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace Billiard.Models;

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