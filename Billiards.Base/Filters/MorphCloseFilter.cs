using OpenCvSharp;

namespace Billiards.Base.Filters;

public class MorphCloseFilter : AbstractFilter
{
    public MorphCloseFilter(AbstractFilter filter) : base(filter)
    {
        Name = "Morph Close";
    }

    protected override void ApplyFilter(Mat originalImage)
    {
        if (ResultMat != null)
        {
            Mat kernelOp = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(3, 3), new Point(-1, -1));
            Cv2.MorphologyEx(GetInputMat(), ResultMat, MorphTypes.Close, kernelOp, new Point(-1, -1),
                1, BorderTypes.Default, new Scalar());
        }
    }
}