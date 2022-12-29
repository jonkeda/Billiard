using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace Billiard.Models;

public class AndFilter : AbstractFilter
{
    public AndFilter(AbstractFilter filter) : base(filter)
    {
        Name = "And";
    }

    private IMaskFilter maskFilter;
    public IMaskFilter MaskFilter
    {
        get { return maskFilter; }
        set { SetProperty(ref maskFilter, value); }
    }

    private IContourFilter contourFilter;
    public IContourFilter ContourFilter
    {
        get { return contourFilter; }
        set { SetProperty(ref contourFilter, value); }
    }


    public int MaskContour { get; set; }

    protected override void ApplyFilter(Mat originalImage)
    {
        Mat input = GetInputMat();
        ResultMat = new Mat();
        if (maskFilter?.Mask?.GetData() != null)
        {
            CvInvoke.BitwiseAnd(input, input, ResultMat, MaskFilter.Mask);
        }
        else if (ContourFilter?.Contours != null)
        {
            Mat mask = Mat.Zeros(input.Rows, input.Cols,DepthType.Cv8U, 1);
            Contour contour = ContourFilter.Contours[MaskContour];
            if (contour.RotatedRectangle.HasValue)
            {
                var rect =  contour.RotatedRectangle.Value;
                rect.Angle += 90;
                CvInvoke.Ellipse(mask, rect, new MCvScalar(255), -1, LineType.Filled);
            }
            else
            {
                CvInvoke.FillPoly(mask, ContourFilter.Contours[MaskContour].AsVectorOfPoint(), new MCvScalar(255));
            }
            CvInvoke.BitwiseAnd(input, input, ResultMat, mask);
        }
        else
        {
            CvInvoke.BitwiseAnd(input, input, ResultMat);
        }
    }
}