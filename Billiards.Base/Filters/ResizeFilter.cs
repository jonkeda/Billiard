    using OpenCvSharp;

    namespace Billiards.Base.Filters;

public class ResizeFilter : AbstractFilter
{
    public ResizeFilter(AbstractFilter filter, float sizing) : base(filter)
    {
        Name = "Resize";
        Sizing = sizing;
    }

    public float Sizing { get; set; }

    protected override void ApplyFilter(Mat originalImage)
    {
        var mat = GetInputMat();
        if (mat == null)
        {
            return;
        }
        Cv2.Resize(mat, ResultMat!, new Size(mat.Width * Sizing, mat.Height * Sizing));
    }
}