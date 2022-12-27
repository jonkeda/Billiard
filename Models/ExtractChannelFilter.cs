using Emgu.CV;

namespace Billiard.Models;

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
        CvInvoke.ExtractChannel(GetInputMat(), ResultMat, Channel);

    }
}