using OpenCvSharp;

namespace Billiards.Base.Filters;

public class FloodFillCornersFilter : AbstractFilter
{
    public int FloodFillColor { get; }

    public FloodFillCornersFilter(AbstractFilter filter, int floodFillColor) : base(filter)
    {
        Name = "FloodFill Corners";
        FloodFillColor = floodFillColor;
    }

    protected override void ApplyFilter(Mat originalImage)
    {
        Mat input = GetInputMat();
        ResultMat = input.Clone();

        Scalar newColor = new Scalar(FloodFillColor);
        float floodFillDiff = 0;
        Scalar diff = new Scalar(floodFillDiff, floodFillDiff, floodFillDiff);
        Mat mask = new Mat();
        Cv2.FloodFill(ResultMat, mask, new Point(0, 0), newColor, 
            out _, diff, diff, FloodFillFlags.FixedRange);
        Cv2.FloodFill(ResultMat, mask, new Point(0, ResultMat.Rows - 1), newColor, 
            out _, diff, diff, FloodFillFlags.FixedRange);
        Cv2.FloodFill(ResultMat, mask, new Point(ResultMat.Cols - 1, ResultMat.Rows - 1), newColor, 
            out _, diff, diff, FloodFillFlags.FixedRange);
        Cv2.FloodFill(ResultMat, mask, new Point(ResultMat.Cols - 1, 0), newColor, 
            out _, diff, diff, FloodFillFlags.FixedRange);
    }
}