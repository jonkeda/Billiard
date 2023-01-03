namespace Billiards.Base.Drawings;

public class DrawingImage
{
    public DrawingVisual VisualDrawing { get; }

    public DrawingImage(DrawingVisual visualDrawing)
    {
        VisualDrawing = visualDrawing;
    }

    public ShapeCollection Shapes { get; set; } = new ShapeCollection();
}