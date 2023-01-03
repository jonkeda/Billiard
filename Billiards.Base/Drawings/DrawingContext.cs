using OpenCvSharp;

namespace Billiards.Base.Drawings;

public class DrawingContext : IDisposable
{
    private readonly DrawingVisual drawingVisual;

    public DrawingContext(DrawingVisual drawingVisual)
    {
        this.drawingVisual = drawingVisual;
    }

    private void AddShape(AbstractShape shape)
    {
        drawingVisual.Shapes.Add(shape);
    }

    public void DrawGeometry(Brush brush, Pen pen, Geometry geometry)
    {
        AddShape(new GeometryShape(brush, pen, geometry));
    }

    public void PushClip(RectangleGeometry rectangleGeometry)
    {
        AddShape(new ClipShape(rectangleGeometry));
    }

    public void Close()
    {
    }

    public void Dispose()
    {
    }

    public void DrawRectangle(Brush? brush, Pen? pen, Rect rect)
    {
        AddShape(new RectangleShape(brush, pen, new Rect2f(rect.X, rect.Y, rect.Width, rect.Height)));
    }

    public void DrawRectangle(Brush? brush, Pen? pen, Rect2f rect)
    {
        AddShape(new RectangleShape(brush, pen, rect));
    }

    public void DrawEllipse(Brush? brush, Pen? pen, Point2f center, float radiusX, float radiusY)
    {
        AddShape(new EllipseShape(brush, pen, center, radiusX, radiusY));
    }

    public void DrawText(FormattedText formattedText, Point2f position)
    {
        AddShape(new TextShape(formattedText, position));
    }

    public void DrawLine(Pen pen, Point2f p1, Point2f p2)
    {
        AddShape(new LineShape(pen, p1, p2));
    }
}