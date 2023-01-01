using Emgu.CV;

namespace Billiard.Models;

public class CloneFilter : AbstractFilter
{
    public CloneFilter(AbstractFilter filter) : base(filter)
    {
        Name = "Clone";
    }

    protected override void ApplyFilter(Mat originalImage)
    {
        ResultMat = GetInputMat().Clone();
    }
}