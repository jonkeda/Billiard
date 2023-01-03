using System.Globalization;
using Billiards.Base.Drawings;
using Billiards.Base.Extensions;
using OpenCvSharp;

namespace Billiards.Base.Filters;

public class FindCornersConvexHullFilter : AbstractFilter, IPointsFilter
{
    public FindCornersConvexHullFilter(AbstractFilter filter) : base(filter)
    {
        Name = "Find Corners Convex Hull";
    }

    private List<Point2f> points;
    public List<Point2f> Points
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
        public Line(Vec2f v1, Vec2f v2)
        {
            V1 = v1;
            V2 = v2;
            V = v1 - v2;

            Length = V.Length();
            Angle = V.Normalize().GetAngle();
        }
        public Vec2f V1 { get;  }
        public Vec2f V2 { get;  }
        public Vec2f V { get;  }

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

        //Rectangle bounds =  Cv2.BoundingRectangle(contourFilter.Contours[0].Points.AsPointArray());
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

        List<Point2f> foundPoints = FindCornerPoints(lines);

        foundPoints = OrderPoints(foundPoints);
        
        Draw(dc =>
        {
            Pen pen = new Pen(Brushes.Red, 2);

            foreach (Line line in lines)
            {
                dc.DrawLine(pen, line.V1, line.V2);
                dc.DrawEllipse(Brushes.Red, null, line.V1.AsPoint2f(), 5, 5);
                dc.DrawEllipse(Brushes.Red, null, line.V2.AsPoint2f(), 5, 5);
            }
        
/*            DrawLine(dc, top, Brushes.Green);
            DrawLine(dc, left, Brushes.DarkBlue);
            DrawLine(dc, right, Brushes.LightBlue);
            DrawLine(dc, bottom, Brushes.LawnGreen);
*/

            int i = 0;
            foreach (Point2f p in foundPoints)
            {
                dc.DrawEllipse(Brushes.Blue, null, p, 5, 5);
                FormattedText formattedText = new(
                    i.ToString(),
                    //CultureInfo.CurrentUICulture,
                    //FlowDirection.LeftToRight,
                    //new Typeface("Verdana"),
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
        Pen pen = new Pen(brush, 3);
        dc.DrawLine(pen, line.V1.AsPoint2f(), line.V2.AsPoint2f());
        const int radius = 10;
        dc.DrawEllipse(brush, null, line.V1.AsPoint2f(), radius, radius);
        dc.DrawEllipse(brush, null, line.V2.AsPoint2f(), radius, radius);

    }


    private Line? FindLine(List<Line> lines, double x1, double x2, double y1, double y2)
    {
        return lines.Where(p =>
            Between(p.V1.X(), x1, x2)
            && Between(p.V2.X(), x1, x2)
            && Between(p.V1.Y(), y1, y2)
            && Between(p.V2.Y(), y1, y2)

        ).MaxBy(p => p.Length);
    }

    private bool Between(double value, double a1, double a2)
    {
        return value >= a1 && value <= a2;
    }


    private List<Point2f> OrderPoints(List<Point2f> foundPoints)
    {
        Point2f point = foundPoints.OrderBy(p => p.X).Take(2).OrderBy(p => p.Y).FirstOrDefault();
        int index = foundPoints.IndexOf(point);
        List<Point2f> points = new();
        points.AddRange(foundPoints.Skip(index));
        points.AddRange(foundPoints.Take(index));
        return points;
    }

    private static List<Point2f> FindCornerPoints(List<Line> lines)
    {
        var foundPoints = FindTop4Lines(lines);
        if (Vec2FExtensions.LineIntersect(lines.Last().V1, lines.Last().V2,
                lines.First().V1, lines.First().V2,
                out Vec2f intersection2))
        {
            foundPoints.Add(intersection2.AsPoint2f());
        }

        return foundPoints;
    }

    private static List<Point2f> FindTop4Lines(List<Line> lines)
    {
        Line previousLine = lines[0];
        List<Point2f> foundPoints = new List<Point2f>();
        foreach (Line line in lines.Skip(1))
        {
            if (Vec2FExtensions.LineIntersect(previousLine.V1, previousLine.V2,
                    line.V1, line.V2,
                    out Vec2f intersection))
            {
                foundPoints.Add(intersection.AsPoint2f());
            }

            previousLine = line;
        }

        return foundPoints;
    }

    private List<Line> CreateLines()
    {
        List<Line> lines = new();
        Point2f previousPoint = ContourFilter.Contours[0].Points[0];
        foreach (var point in ContourFilter.Contours[0].Points.Skip(1))
        {
            lines.Add(new Line(previousPoint.AsVec2f(), point.AsVec2f()));
            previousPoint = point;
        }

        lines.Add(new Line(ContourFilter.Contours[0].Points.Last().AsVec2f(),
            ContourFilter.Contours[0].Points.First().AsVec2f()));
        return lines;
    }
}