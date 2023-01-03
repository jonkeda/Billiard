using OpenCvSharp;

namespace Billiards.Base.Filters;

public class ExtractChannelFilter : AbstractFilter
{
    public int Channel { get; }
    public ExtractChannelFilter(AbstractFilter filter, int channel) : base(filter)
    {
        Name = "ExtractChannel";
        Channel = channel;
    }

    protected override void ApplyFilter(Mat originalImage)
    {
        if (ResultMat != null)
        {
            Cv2.ExtractChannel(GetInputMat()!, ResultMat, Channel);
        }
    }
}