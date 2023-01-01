using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace Billiard.Models;

public class MorphOpenFilter : AbstractFilter
{
    public MorphOpenFilter(AbstractFilter filter) : base(filter)
    {
        Name = "Morph Open";
    }

    protected override void ApplyFilter(Mat originalImage)
    {
        Mat kernelOp = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(3, 3), new Point(-1, -1));
        CvInvoke.MorphologyEx(GetInputMat(), ResultMat, MorphOp.Open, kernelOp, new Point(-1, -1), 
            1, BorderType.Default, new MCvScalar());
    }
}