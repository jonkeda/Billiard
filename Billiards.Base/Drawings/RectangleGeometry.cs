using OpenCvSharp;

namespace Billiards.Base.Drawings;

public class RectangleGeometry : Geometry
{
    public Rect2f Rectangle { get; set; }
    public RectangleGeometry(Rect2f rect)
    {
        Rectangle = rect;
    }
}