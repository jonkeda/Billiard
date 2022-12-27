using System.Windows.Media;
using Billiard.Camera.vision.Geometries;
using Emgu.CV;

namespace Billiard.Models;

public class DrawBoundingRectFilter : AbstractFilter
{
    private IBoundingRectFilter boundingRect;

    public DrawBoundingRectFilter(AbstractFilter filter) : base(filter)
    {
        Name = "Draw Bounding Rectangle";
    }

    public IBoundingRectFilter BoundingRect
    {
        get
        {
            if (boundingRect == null)
            {
                return InputFilter as IBoundingRectFilter;
            }
            return boundingRect;
        }
        set { boundingRect = value; }
    }
    
    protected override void ApplyFilter(Mat originalImage)
    {
        ResultMat = GetInputMat();
        if (BoundingRect is IBoundingRectFilter boundingRectFilter)
        {
            Pen color = new Pen(Brushes.GreenYellow, Math.max(ResultMat.Cols / 100, ResultMat.Rows / 100))
            {
                DashStyle = DashStyles.Solid
            };

            Draw(dc =>
                dc.DrawRectangle(null, color, boundingRectFilter.BoundingRect.AsRect()));
        }
    }
}