using OpenCvSharp;

namespace Billiards.Base.Filters;

public class OriginalFilter : AbstractFilter
{
    private Mat inputMat = new Mat();
    public Mat InputMat
    {
        get { return inputMat; }
        set { SetProperty(ref inputMat, value); }
    }

    public OriginalFilter() : base(null)
    {
        Name = "Original";
    }

    protected override void ApplyFilter(Mat originalImage)
    {
        InputMat = originalImage;
        ResultMat = originalImage;
    }
}