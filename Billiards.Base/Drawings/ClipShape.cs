namespace Billiards.Base.Drawings;

public class ClipShape : AbstractShape
{
    public RectangleGeometry RectangleGeometry { get; }

    public ClipShape(RectangleGeometry rectangleGeometry)
    {
        RectangleGeometry = rectangleGeometry;
    }

    public override void Render(IRenderer wpfRenderer)
    {
        wpfRenderer.PushClip(RectangleGeometry);
    }
}