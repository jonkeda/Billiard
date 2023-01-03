namespace Billiards.Base.Drawings;

public abstract class AbstractShape
{
    protected AbstractShape(){}

    protected AbstractShape(Brush? brush, Pen? pen)
    {
        Brush = brush;
        Pen = pen;
    }

    public Brush? Brush { get; }
    public Pen? Pen { get; }

    public virtual void Render(IRenderer wpfRenderer)
    {
            
    }
}