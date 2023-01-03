using OpenCvSharp;

namespace Billiards.Base.Drawings;

public class RectangleShape : AbstractShape
{
    public Rect2f Rect { get; }

    public RectangleShape(Brush? brush, Pen? pen, Rect2f rect) : base(brush, pen)
    {
        Rect = rect;
    }
}