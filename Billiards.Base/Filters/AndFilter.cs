using OpenCvSharp;

namespace Billiards.Base.Filters;

public class AndFilter : AbstractFilter, IMaskFilter
{
    public AndFilter(AbstractFilter filter) : base(filter)
    {
        Name = "And";
    }

    private Mat? mask = new();
    public Mat? Mask
    {
        get { return mask; }
        set
        {
            SetProperty(ref mask, value);
        }
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
        ResultMat = new Mat(input.Rows, input.Cols, input.Type());
        if (MaskFilter?.Mask != null)
        {
            Cv2.BitwiseAnd(input, input, ResultMat, MaskFilter.Mask);
        }
        else if (ContourFilter?.Contours != null)
        {
            mask = new Mat(input.Rows, input.Cols, MatType.CV_8U, new Scalar(0));
            Contour contour = ContourFilter.Contours[MaskContour];
            if (contour.RotatedRectangle.HasValue)
            {
                var rect =  contour.RotatedRectangle.Value;
                Cv2.Ellipse(mask, rect, new Scalar(255), -1);
            }
            else
            {
                List<List<Point>> lists = new List<List<Point>>
                {
                    ContourFilter.Contours[MaskContour].Points
                };

                Cv2.FillPoly(mask, lists, new Scalar(255), LineTypes.Link4);
            }
            // input.BitwiseAnd()
            Cv2.BitwiseAnd(input, input, ResultMat, mask);
        }
        else
        {
            Cv2.BitwiseAnd(input, input, ResultMat);
        }

        //ResultMat = GetInputMat();
    }
}