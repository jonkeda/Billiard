using System.Collections.Generic;
using System.Drawing;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Media;
using Billiard.Camera.vision.Geometries;
using Billiard.Utilities;
using Emgu.CV;
using static System.Windows.Media.Brushes;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Pen = System.Windows.Media.Pen;
using Point = System.Drawing.Point;

namespace Billiard.Models;

public class FindCornersConvexHullFilter : AbstractFilter, IPointsFilter
{
    public FindCornersConvexHullFilter(AbstractFilter filter) : base(filter)
    {
        Name = "Find Corners Convex Hull";
    }

    private List<System.Windows.Point> points;
    public List<System.Windows.Point> Points
    {
        get { return points; }
        set { SetProperty(ref points, value); }
    }


    private IContourFilter contourFilter;
    public IContourFilter ContourFilter
    {
        get { return contourFilter; }
        set { SetProperty(ref contourFilter, value); }
    }

    public class Line
    {
        public Line(Vector2 v1, Vector2 v2)
        {
            V1 = v1;
            V2 = v2;
            V = v1 - v2;

            Length = V.Length();
            Angle = V.Normalize().GetAngle();
        }
        public Vector2 V1 { get;  }
        public Vector2 V2 { get;  }
        public Vector2 V { get;  }

        public float Length { get; }
        public float Angle { get; }

    }

    protected override void ApplyFilter(Mat originalImage)
    {
        ResultMat = GetInputMat();

        if (ContourFilter?.Contours == null
            || ContourFilter.Contours.Count == 0
            || ContourFilter.Contours[0].Points.Count < 4)
        {
            return;
        }

        //Rectangle bounds =  CvInvoke.BoundingRectangle(contourFilter.Contours[0].Points.AsPointArray());
        var lines = CreateLines();

        //List<Line> foundLines = new();

/*        var top = FindLine(lines, bounds.Left, bounds.Right, 
            bounds.Top, bounds.Top + bounds.Height * 0.25d);
        if (top != null)
        {
            foundLines.Add(top);
        }
        var right = FindLine(lines, 
            bounds.Right - bounds.Width * 0.5d, bounds.Right,
            bounds.Top, bounds.Bottom);
        if (right != null)
        {
            foundLines.Add(right);
        }

        var bottom = FindLine(lines, 
            bounds.Left, bounds.Right, 
            bounds.Bottom - bounds.Height * 0.25d, bounds.Bottom);
        if (bottom != null)
        {
            foundLines.Add(bottom);
        }
        var left = FindLine(lines, bounds.Left, bounds.Left + bounds.Width * 0.5d,
            bounds.Top, bounds.Bottom);
        if (left != null)
        {
            foundLines.Add(left);
        }
*/


        lines = lines.OrderByDescending(l => l.Length).Take(4).ToList();
        lines = lines.OrderBy(l => l.Angle).ToList();

        List<System.Windows.Point> foundPoints = FindCornerPoints(lines);

        foundPoints = OrderPoints(foundPoints);

        System.Windows.Media.Pen pen = new Pen(Brushes.Red, 2);
        Draw(dc =>
        {

            foreach (Line line in lines)
            {
                dc.DrawLine(pen, PointFExtensions.AsPoint(line.V1), PointFExtensions.AsPoint(line.V2));
                dc.DrawEllipse(Brushes.Red, null, PointFExtensions.AsPoint(line.V1), 5, 5);
                dc.DrawEllipse(Brushes.Red, null, PointFExtensions.AsPoint(line.V2), 5, 5);
            }

/*            DrawLine(dc, top, Brushes.Green);
            DrawLine(dc, left, Brushes.DarkBlue);
            DrawLine(dc, right, Brushes.LightBlue);
            DrawLine(dc, bottom, Brushes.LawnGreen);
*/

            int i = 0;
            foreach (System.Windows.Point p in foundPoints)
            {
                dc.DrawEllipse(Brushes.Blue, null, p, 5, 5);
                FormattedText formattedText = new(
                    i.ToString(),
                    CultureInfo.CurrentUICulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Verdana"),
                    32,
                    Brushes.GreenYellow, 1.25);
                dc.DrawText(formattedText, p);
                i++;
            }
        });

        Points = foundPoints;
    }

    private void DrawLine(DrawingContext dc, Line line, Brush brush)
    {
        if (line == null)
        {
            return;
        }
        System.Windows.Media.Pen pen = new Pen(brush, 3);
        dc.DrawLine(pen, PointFExtensions.AsPoint(line.V1), PointFExtensions.AsPoint(line.V2));
        const int radius = 10;
        dc.DrawEllipse(brush, null, PointFExtensions.AsPoint(line.V1), radius, radius);
        dc.DrawEllipse(brush, null, PointFExtensions.AsPoint(line.V2), radius, radius);

    }

    private Line FindLine(List<Line> lines, double x1, double x2, double y1, double y2)
    {
        return lines.Where(p =>
            Between(p.V1.X, x1, x2)
            && Between(p.V2.X, x1, x2)
            && Between(p.V1.Y, y1, y2)
            && Between(p.V2.Y, y1, y2)

        ).MaxBy(p => p.Length);
    }

    private bool Between(double value, double a1, double a2)
    {
        return value >= a1 && value <= a2;
    }


    private List<System.Windows.Point> OrderPoints(List<System.Windows.Point> foundPoints)
    {
        System.Windows.Point point = foundPoints.OrderBy(p => p.X).Take(2).OrderBy(p => p.Y).FirstOrDefault();
        int index = foundPoints.IndexOf(point);
        List<System.Windows.Point> points = new();
        points.AddRange(foundPoints.Skip(index));
        points.AddRange(foundPoints.Take(index));
        return points;
    }

    private static List<System.Windows.Point> FindCornerPoints(List<Line> lines)
    {
        var foundPoints = FindTop4Lines(lines);
        if (PointFExtensions.LineIntersect(lines.Last().V1, lines.Last().V2,
                lines.First().V1, lines.First().V2,
                out Vector2 intersection2))
        {
            foundPoints.Add(PointFExtensions.AsPoint(intersection2));
        }

        return foundPoints;
    }

    private static List<System.Windows.Point> FindTop4Lines(List<Line> lines)
    {
        Line previousLine = lines[0];
        List<System.Windows.Point> foundPoints = new List<System.Windows.Point>();
        foreach (Line line in lines.Skip(1))
        {
            if (PointFExtensions.LineIntersect(previousLine.V1, previousLine.V2,
                    line.V1, line.V2,
                    out Vector2 intersection))
            {
                foundPoints.Add(PointFExtensions.AsPoint(intersection));
            }

            previousLine = line;
        }

        return foundPoints;
    }

    private List<Line> CreateLines()
    {
        List<Line> lines = new();
        Point previousPoint = ContourFilter.Contours[0].Points[0];
        foreach (var point in ContourFilter.Contours[0].Points.Skip(1))
        {
            lines.Add(new Line(previousPoint.AsVector2(), point.AsVector2()));
            previousPoint = point;
        }

        lines.Add(new Line(ContourFilter.Contours[0].Points.Last().AsVector2(),
            ContourFilter.Contours[0].Points.First().AsVector2()));
        return lines;
    }
}