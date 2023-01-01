using OpenCvSharp;

namespace Billiards.Base.Filters;

public class MorphOpenFilter : AbstractFilter
{
    public MorphOpenFilter(AbstractFilter filter) : base(filter)
    {
        Name = "Morph Open";
    }

    protected override void ApplyFilter(Mat originalImage)
    {
        Mat kernelOp = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(3, 3), new Point(-1, -1));
        Cv2.MorphologyEx(GetInputMat(), ResultMat, MorphTypes.Open, kernelOp, new Point(-1, -1), 
            1, BorderTypes.Default, new Scalar());
    }
}