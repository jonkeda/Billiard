namespace Billiards.Base.Drawings;

public class DrawingVisual
{
    public ShapeCollection Shapes { get; set; } = new ShapeCollection();

    public DrawingVisual Drawing
    {
        get
        {
            return this;
        }
    }

    public DrawingContext RenderOpen()
    {
        return new DrawingContext(this);
    }
}