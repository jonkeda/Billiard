using OpenCvSharp;

namespace Billiards.Base.Filters;

public class MorphOpenFilter : AbstractFilter
{
    private MorphTypes morphType;
    public MorphOpenFilter(AbstractFilter filter) : this(filter, MorphTypes.Open)
    {
        Name = "Morph Open";
    }

    public MorphOpenFilter(AbstractFilter filter, MorphTypes morphType) : base(filter)
    {
        Name = "Morph Open";
        this.morphType = morphType;
    }

    public MorphShapes MorphShapes { get; set; } = MorphShapes.Rect;
    public Size Size { get; set; } = new Size(3, 3);
    public Point Anchor { get; set; } = new Point(-1, -1);

    protected override void ApplyFilter(Mat originalImage)
    {
        FilterValues.Add("MorphShapes", MorphShapes.ToString());
        FilterValues.Add("Size", Size.ToString());
        FilterValues.Add("Anchor", Anchor.ToString());

        if (ResultMat != null)
        {
            Mat kernelOp = Cv2.GetStructuringElement(MorphShapes, Size, Anchor);
            Cv2.MorphologyEx(GetInputMat()!, ResultMat, morphType, kernelOp, new Point(-1, -1),
                1, BorderTypes.Default, new Scalar());
        }
    }
}