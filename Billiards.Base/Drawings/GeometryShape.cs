namespace Billiards.Base.Drawings;

public class GeometryShape : AbstractShape
{
    public Geometry Geometry { get; }

    public GeometryShape(Brush brush, Pen pen, Geometry geometry) : base(brush, pen)
    {
        Geometry = geometry;
    }
}