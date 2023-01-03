using OpenCvSharp;

namespace Billiards.Base.Drawings;

public class EllipseShape : AbstractShape
{
    public Point2f Center { get; }
    public float RadiusX { get; }
    public float RadiusY { get; }

    public EllipseShape(Brush? brush, Pen? pen, Point2f center, float radiusX, float radiusY) : base(brush, pen)
    {
        Center = center;
        RadiusX = radiusX;
        RadiusY = radiusY;
    }

    public override void Render(IRenderer wpfRenderer)
    {
        wpfRenderer.DrawEllipse(Brush, Pen, Center, RadiusX, RadiusY);
    }
}