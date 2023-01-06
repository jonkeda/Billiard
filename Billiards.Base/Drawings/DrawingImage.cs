namespace Billiards.Base.Drawings;

public class DrawingImage
{
    public DrawingImage(DrawingVisual visualDrawing)
    {
        Shapes = visualDrawing.Shapes;
    }

    public ShapeCollection Shapes { get; set; }

    public IPlatformDrawingImage? PlatformDrawingImage { get; set; }
}