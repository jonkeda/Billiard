using System.Collections.ObjectModel;
using OpenCvSharp;

namespace Billiards.Base.Drawings
{
    public enum DashStyles
    {
        Solid,
        Dash,
        Dot
    }

    public class Pen
    {
        public Pen(Brush brush, int thickness)
        {
            Brush = brush;
            Thickness = thickness;
        }

        public int Thickness { get; set; }

        public Brush Brush { get; set; }

        public DashStyles DashStyle { get; set; }
    }

    public static class Brushes
    {
        public static Brush GreenYellow { get; } = new(Colors.GreenYellow);
        public static Brush Red { get; } = new(Colors.Red);
    }

    public class Brush
    {
        public Brush(Color color)
        {
            Color = Color;
        }

        public Color Color { get; set; }
    }

    public static class Colors
    {
        public static Color GreenYellow { get; } = new ();
        public static Color Red { get; } = new();
    }

    public class Color
    {
        public static Color FromArgb(int a, int r, int g, int b)
        {
            return new Color();
        }
    }

    public class ShapeCollection : Collection<AbstractShape>
    {
    }

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
    }

    public class PathFigure
    {
        public bool IsClosed { get; set; }
        public Point2f? StartPoint { get; set; }
        public LineSegmentCollection Segments { get; } = new ();
    }

    public class LineSegmentCollection : Collection<LineSegment>
    {}

    public class LineSegment
    {
        public Point2f Point { get; }
        public bool IsStroked { get; }

        public LineSegment(Point2f point, bool isStroked)
        {
            Point = point;
            IsStroked = isStroked;
        }
    }

    public abstract class Geometry
    {

    }

    public class GeometryShape : AbstractShape
    {
        public Geometry Geometry { get; }

        public GeometryShape(Brush brush, Pen pen, Geometry geometry) : base(brush, pen)
        {
            Geometry = geometry;
        }
    }

    public class PathGeometry : Geometry
    {
        public PathGeometry(List<PathFigure> pathFigures)
        {
            PathFigures = pathFigures;
        }

        private List<PathFigure> PathFigures { get; set; }
    }

    public class RectangleGeometry : Geometry
    {
        public Rect2f Rectangle { get; set; }
        public RectangleGeometry(Rect2f rect)
        {
            Rectangle = rect;
        }
    }

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

    public class DrawingImage
    {
        public DrawingVisual VisualDrawing { get; }

        public DrawingImage(DrawingVisual visualDrawing)
        {
            VisualDrawing = visualDrawing;
        }

        public ShapeCollection Shapes { get; set; } = new ShapeCollection();
    }

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

        public void DrawRectangle(Brush brush, Pen pen, Rect rect)
        {
            AddShape(new RectangleShape(brush, pen, rect));
        }

        public void DrawEllipse(Brush brush, Pen pen, Point2f center, float radiusX, float radiusY)
        {
            AddShape(new DrawnEllipse(brush, pen, center, radiusX, radiusY));
        }
    }

    public class DrawnEllipse : AbstractShape
    {
        public Point2f Center { get; }
        public float RadiusX { get; }
        public float RadiusY { get; }

        public DrawnEllipse(Brush brush, Pen pen, Point2f center, float radiusX, float radiusY) : base(brush, pen)
        {
            Center = center;
            RadiusX = radiusX;
            RadiusY = radiusY;
        }
    }

    public class RectangleShape : AbstractShape
    {
        public Rect Rect { get; }

        public RectangleShape(Brush brush, Pen pen, Rect rect) : base(brush, pen)
        {
            Rect = rect;
        }
    }

    public class SolidColorBrush : Brush
    {
        public SolidColorBrush(Color color) : base(color)
        {
        }
    }

    public class ClipShape : AbstractShape
    {
        public RectangleGeometry RectangleGeometry { get; }

        public ClipShape(RectangleGeometry rectangleGeometry)
        {
            RectangleGeometry = rectangleGeometry;
        }
    }
}
