using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using Billiards.Base.Drawings;
using OpenCvSharp;
using Brush = Billiards.Base.Drawings.Brush;
using Brushes = Billiards.Base.Drawings.Brushes;
using Color = Billiards.Base.Drawings.Color;
using DashStyles = Billiards.Base.Drawings.DashStyles;
using FormattedText = Billiards.Base.Drawings.FormattedText;
using Geometry = Billiards.Base.Drawings.Geometry;
using LineSegment = Billiards.Base.Drawings.LineSegment;
using PathFigure = Billiards.Base.Drawings.PathFigure;
using PathGeometry = Billiards.Base.Drawings.PathGeometry;
using Pen = Billiards.Base.Drawings.Pen;
using Point = System.Windows.Point;
using Rect = System.Windows.Rect;
using RectangleGeometry = Billiards.Base.Drawings.RectangleGeometry;

namespace Billiards.Wpf.Drawings;

public class WpfRenderer : IRenderer
{
    private System.Windows.Media.DrawingContext DrawingContext { get; }

    public WpfRenderer(System.Windows.Media.DrawingContext drawingContext)
    {
        DrawingContext = drawingContext;
    }

    public void DrawEllipse(Brush? brush, Pen? pen, Point2f center, float radiusX, float radiusY)
    {
        DrawingContext.DrawEllipse(MakeBrush(brush), MakePen(pen), MakePoint(center), radiusX, radiusY);
    }

    public void PushClip(Geometry rectangleGeometry)
    {
        DrawingContext.PushClip(MakeGeometry(rectangleGeometry));
    }

    private System.Windows.Media.Geometry MakeGeometry(Geometry geometry)
    {

        if (geometry.PlatformGeometry is WpfPlatformGeometry wpfPlatformGeometry)
        {
            return wpfPlatformGeometry.Geometry;
        }

        System.Windows.Media.Geometry? wpfGeometry = null;
        if (geometry is RectangleGeometry rectangleGeometry)
        {
            wpfGeometry = 
                new System.Windows.Media.RectangleGeometry(MakeRect(rectangleGeometry.Rectangle));
        }
        if (geometry is PathGeometry pathGeometry)
        {
            wpfGeometry =
                new System.Windows.Media.PathGeometry(MakePathFigures(pathGeometry.PathFigures));
        }

        if (wpfGeometry == null)
        {
            throw new Exception("Geometry not defined.");
        }
        wpfPlatformGeometry = new WpfPlatformGeometry(wpfGeometry);

        geometry.PlatformGeometry = wpfPlatformGeometry;

        return wpfGeometry;
    }

    public void DrawRectangle(Brush? brush, Pen? pen, Rect2f rect)
    {
        DrawingContext.DrawRectangle(MakeBrush(brush), MakePen(pen), MakeRect(rect) );
    }

    public void DrawLine(Pen? pen, Point2f p1, Point2f p2)
    {
        DrawingContext.DrawLine(MakePen(pen), MakePoint(p1), MakePoint(p2));
    }

    public void DrawText(FormattedText formattedText, Point2f position)
    {
        DrawingContext.DrawText(MakeFormattedText(formattedText), MakePoint(position));
    }

    public void DrawGeometry(Brush? brush, Pen? pen, Geometry geometry)
    {
        DrawingContext.DrawGeometry(MakeBrush(brush), MakePen(pen), MakeGeometry(geometry));
    }

    private System.Windows.Media.FormattedText MakeFormattedText(FormattedText formattedText)
    {
        if (formattedText.PlatformFormattedText is WpfPlatformFormattedText wpfFormattedText)
        {
            return wpfFormattedText.FormattedText;
        }

        System.Windows.Media.FormattedText newFormattedText 
            = new System.Windows.Media.FormattedText(
                formattedText.Text,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                new System.Windows.Media.Typeface("Verdana"),
                MakeSize(formattedText.Size),
                MakeBrush(formattedText.Brush), formattedText.PointSize);

        wpfFormattedText = new WpfPlatformFormattedText(newFormattedText);

        formattedText.PlatformFormattedText = wpfFormattedText;

        return wpfFormattedText.FormattedText;
    }

    private double MakeSize(int formattedTextSize)
    {
        return formattedTextSize;
    }

    private List<System.Windows.Media.PathFigure> MakePathFigures(List<PathFigure> pathFigures)
    {
        List<System.Windows.Media.PathFigure> wpfPathFigures = new();

        foreach (PathFigure figure in pathFigures)
        {
            System.Windows.Media.PathFigure wpfPathFigure = new()
            {
                IsClosed = figure.IsClosed,
                IsFilled = figure.IsClosed,
                StartPoint = MakePoint(figure.StartPoint),
                Segments = MakePathSegments(figure.Segments)
            };
            wpfPathFigures.Add(wpfPathFigure);
        }

        return wpfPathFigures;
    }

    private PathSegmentCollection MakePathSegments(LineSegmentCollection lineSegments)
    {
        PathSegmentCollection pathSegments = new();

        foreach (LineSegment line in lineSegments)
        {
           pathSegments.Add(new System.Windows.Media.LineSegment(MakePoint(line.Point), line.IsStroked)); 
        }

        return pathSegments;
    }


    private Rect MakeRect(Rect2f rect)
    {
        return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
    }

    private Point MakePoint(Point2f point)
    {
        return new Point(point.X, point.Y);
    }

    private System.Windows.Media.Pen? MakePen(Pen? pen)
    {
        if (pen == null)
        {
            return null;
        }
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

    private System.Windows.Media.Brush? MakeBrush(Brush? brush)
    {
        if (brush == null)
        {
            return null;
        }

        if (brush.PlatformBrush is WpfPlatformBrush wpfBrush)
        {
            return wpfBrush.Brush;
        }

        var color = MakeColor(brush.Color);
        if (!color.HasValue)
        {
            return null;
        }

        System.Windows.Media.Brush newBrush = new System.Windows.Media.SolidColorBrush(color.Value);


        wpfBrush = new WpfPlatformBrush(newBrush);

        brush.PlatformBrush = wpfBrush;

        return wpfBrush.Brush;
    }

    private System.Windows.Media.Color? MakeColor(Color? color)
    {
        if (color == null)
        {
            return null;
        }
        if (color.PlatformColor is WpfPlatformColor wpfColor)
        {
            return wpfColor.Color;
        }

        var c = color;
        System.Windows.Media.Color newColor =  
            System.Windows.Media.Color.FromArgb(c.A, c.R, c.G, c.B);


        wpfColor = new WpfPlatformColor(newColor);

        color.PlatformColor = wpfColor;

        return wpfColor.Color;
    }

}