using System.Drawing;
using Emgu.CV;

namespace Billiard.Models;

public class GaussianBlurFilter : AbstractFilter
{
    public GaussianBlurFilter(AbstractFilter filter) : base(filter)
    {
        Name = "Gausian";
    }

    protected override void ApplyFilter(Mat originalImage)
    {

        CvInvoke.GaussianBlur(GetInputMat(), ResultMat, new Size(3, 3), 1);
    }
}