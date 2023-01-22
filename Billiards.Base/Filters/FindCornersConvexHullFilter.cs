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

    public int StraigthenAngle { get; set; }

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
            || ContourFilter.Contours.Count == 0)
        {
            return;
        }

        var contour = ContourFilter.Contours[0];
        if (contour.Points == null)
        {
            return;
        }

        List<Point> newPoints = StraightenLines(contour.Points);
        var lines = CreateLines(newPoints);

        int i = 0;
        FilterValues.Add("Width", ResultMat.Width);
        FilterValues.Add("Height", ResultMat.Height);
        foreach (Line line in lines)
        {
            FilterValues.Add(i.ToString(), $"{line.V1.Item0:F0} {line.V1.Item1:F0} {line.V2.Item0:F0} {line.V2.Item1:F0} ");
            //FilterValues.Add(i.ToString(), Math.Round(line.Angle));
            i++;
        }

        var allLines = lines.ToList();

        lines = FilterSideLines(lines, ResultMat.Width, ResultMat.Height);

        lines = lines.OrderByDescending(l => l.Length).Take(4).ToList();
        lines = lines.OrderBy(l => l.Angle).ToList();

        List<Point2f> foundPoints = FindCornerPoints(lines);

        foundPoints = OrderPoints(foundPoints);
        
        Draw(dc =>
        {
            float radius = Math.Max(ResultMat.Cols, ResultMat.Rows) / 50;

            Pen penAll = new Pen(Brushes.GreenYellow, (int)(radius / 2));
            foreach (Line line in allLines)
            {
                dc.DrawLine(penAll, line.V1, line.V2);
                dc.DrawEllipse(Brushes.GreenYellow, null, line.V1.AsPoint2f(), radius, radius);
                dc.DrawEllipse(Brushes.GreenYellow, null, line.V2.AsPoint2f(), radius, radius);
            }

            Pen pen = new Pen(Brushes.Red, (int)(radius / 2));
            foreach (Line line in lines)
            {
                dc.DrawLine(pen, line.V1, line.V2);
                dc.DrawEllipse(Brushes.Red, null, line.V1.AsPoint2f(), radius, radius);
                dc.DrawEllipse(Brushes.Red, null, line.V2.AsPoint2f(), radius, radius);
            }

            int i = 0;
            foreach (Point2f p in foundPoints)
            {
                dc.DrawEllipse(Brushes.Blue, null, p, radius, radius);
                FormattedText formattedText = new(
                    i.ToString(),
                    32,
                    Brushes.GreenYellow, 1.25);
                dc.DrawText(formattedText, p);
                i++;
            }
        });
    
        Points = foundPoints;
    }

    private List<Line> FilterSideLines(List<Line> lines, int width, int height)
    {
        width--;
        height--;
        List<Line> filteredLines = new();
        foreach (var line in lines)
        {
            if ((line.V1.Item0 <= 1 && line.V2.Item0 <= 1)
                || (line.V1.Item1 <= 1 && line.V2.Item1 <= 1)
                || (line.V1.Item0 >= width && line.V2.Item0 >= width)
                || (line.V1.Item1 >= height && line.V2.Item1 >= height))
            {

            }
            else
            {
                filteredLines.Add(line);
            }

        }
        return filteredLines;
    }


    private static List<Point2f> OrderPoints(List<Point2f> foundPoints)
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

    private List<Point> StraightenLines(List<Point> points)
    {
        List<Point> newPoints = new();
        Point previousPoint = points[0];
        float angle = float.MaxValue;
        newPoints.Add(previousPoint);
        foreach (var point in points.Skip(1))
        {
            var line = new Line(previousPoint.AsVec2f(), point.AsVec2f());
            if (Math.Abs(angle - line.Angle) > StraigthenAngle)
            {
                newPoints.Add(previousPoint);
                angle = line.Angle;
            }
            previousPoint = point;
        }
        newPoints.Add(previousPoint);

        return newPoints;
    }

    private static List<Line> CreateLines(IReadOnlyList<Point> points)
    {
        List<Line> lines = new();
        Point previousPoint = points[0];
        foreach (var point in points.Skip(1))
        {
            lines.Add(new Line(previousPoint.AsVec2f(), point.AsVec2f()));
            previousPoint = point;
        }
        lines.Add(new Line(points.Last().AsVec2f(),
            points.First().AsVec2f()));
        return lines;
    }
}