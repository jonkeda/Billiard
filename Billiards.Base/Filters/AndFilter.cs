using OpenCvSharp;
using OpenCvSharp.Internal.Vectors;

namespace Billiards.Base.Filters;

public class AndFilter : AbstractFilter
{
    public AndFilter(AbstractFilter filter) : base(filter)
    {
        Name = "And";
    }

    private IMaskFilter? maskFilter;
    public IMaskFilter? MaskFilter
    {
        get { return maskFilter; }
        set { SetProperty(ref maskFilter, value); }
    }

    private IContourFilter? contourFilter;
    public IContourFilter? ContourFilter
    {
        get { return contourFilter; }
        set { SetProperty(ref contourFilter, value); }
    }


    public int MaskContour { get; set; }

    protected override void ApplyFilter(Mat originalImage)
    {
        Mat input = GetInputMat();
        ResultMat = new Mat();
        if (maskFilter?.Mask != null)
        {
            Cv2.BitwiseAnd(input, input, ResultMat, MaskFilter.Mask);
        }
        else if (ContourFilter?.Contours != null)
        {
            Mat mask = Mat.Zeros(input.Rows, input.Cols, MatType.CV_8U, 1);
            Contour contour = ContourFilter.Contours[MaskContour];
            if (contour.RotatedRectangle.HasValue)
            {
                var rect =  contour.RotatedRectangle.Value;
                rect.Angle += 90;
                Cv2.Ellipse(mask, rect, new Scalar(255), -1);
            }
            else
            {
                List<List<Point>> lists = new List<List<Point>>();
                lists.Add(ContourFilter.Contours[MaskContour].Points);
                
                Cv2.FillPoly(mask, lists, new Scalar(255));
            }
            Cv2.BitwiseAnd(input, input, ResultMat, mask);
        }
        else
        {
            Cv2.BitwiseAnd(input, input, ResultMat);
        }
    }
}