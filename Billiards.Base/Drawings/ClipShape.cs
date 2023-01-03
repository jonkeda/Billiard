namespace Billiards.Base.Drawings;

public class ClipShape : AbstractShape
{
    public RectangleGeometry RectangleGeometry { get; }

    public ClipShape(RectangleGeometry rectangleGeometry)
    {
        RectangleGeometry = rectangleGeometry;
    }
}