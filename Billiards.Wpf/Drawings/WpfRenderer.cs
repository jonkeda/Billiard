using Billiards.Base.Drawings;
using OpenCvSharp;
using Point = System.Windows.Point;

namespace Billiards.Wpf.Drawings;

public class WpfRenderer : IRenderer
{
    private System.Windows.Media.DrawingContext DrawingContext { get; }

    public WpfRenderer(System.Windows.Media.DrawingContext drawingContext)
    {
        DrawingContext = drawingContext;
    }

    public void DrawEllipse(Brush brush, Pen pen, Point2f center, float radiusX, float radiusY)
    {
        DrawingContext.DrawEllipse(MakeBrush(brush), MakePen(pen), MakePoint(center), radiusX, radiusY);
    }

    private Point MakePoint(Point2f point)
    {
        return new Point(point.X, point.Y);
    }

    private System.Windows.Media.Pen MakePen(Pen pen)
    {
        if (pen.PlatformPen is WpfPlatformPen wpfPen)
        {
            return wpfPen.Pen;
        }

        System.Windows.Media.Pen newPen = new System.Windows.Media.Pen(MakeBrush(pen.Brush), pen.Thickness);

        if (pen.DashStyle == DashStyles.Dash)
        {
            newPen.DashStyle = System.Windows.Media.DashStyles.Dash;
        }
        else if (pen.DashStyle == DashStyles.Dot)
        {
            newPen.DashStyle = System.Windows.Media.DashStyles.Dot;
        }
        else if (pen.DashStyle == DashStyles.Solid)
        {
            newPen.DashStyle = System.Windows.Media.DashStyles.Solid;
        }

        wpfPen = new WpfPlatformPen(newPen);

        pen.PlatformPen = wpfPen;

        return wpfPen.Pen;
    }

    private System.Windows.Media.Brush MakeBrush(Brush brush)
    {
        if (brush.PlatformBrush is WpfPlatformBrush wpfBrush)
        {
            return wpfBrush.Brush;
        }

        System.Windows.Media.Brush newBrush = new System.Windows.Media.SolidColorBrush(MakeColor(brush.Color));


        wpfBrush = new WpfPlatformBrush(newBrush);

        brush.PlatformBrush = wpfBrush;

        return wpfBrush.Brush;
    }

    private System.Windows.Media.Color MakeColor(Color color)
    {
        if (color.PlatformColor is WpfPlatformColor wpfColor)
        {
            return wpfColor.Color;
        }

        // todo
        System.Windows.Media.Color newColor = new System.Windows.Media.Color();


        wpfColor = new WpfPlatformColor(newColor);

        color.PlatformColor = wpfColor;

        return wpfColor.Color;
    }

}