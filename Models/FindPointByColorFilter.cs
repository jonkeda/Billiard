using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Billiard.Camera.vision.Geometries;
using Emgu.CV;
using Point = System.Windows.Point;

namespace Billiard.Models;

public class FindPointByColorFilter : AbstractFilter, IPointFilter
{
    public class FoundPoint
    {
        public FoundPoint(int color, int x, int y)
        {
            Color = color;
            X = x;
            Y = y;
            Add(x, y);
            Count = 1;
        }

        public int Color { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Count { get; set; }
        public List<Point> Points { get; } = new();

        public void Add(int x, int y)
        {
            Count++;
            Points.Add(new Point(x, y));
        }
    }

    public Point Point { get; set; }

    public int Size { get; set; } = 10;
    public int Step { get; set; } = 5;

    public FindPointByColorFilter(AbstractFilter filter) : base(filter)
    {
        Name = "Find Point By Color Filter";
    }

    protected override void ApplyFilter(Mat originalImage)
    {
        
        ResultMat = GetInputMat();

        if (ResultMat?.GetData() == null)
        {
            return;
        }

        int midX = ResultMat.Cols / 2;
        int midY = ResultMat.Rows / 2;

        Dictionary<int, FoundPoint> points = new();
        for (int x = midX - Size; x <= midX + Size; x+=Step)
        {
            for (int y = midY - Size; y <= midY + Size; y += Step)
            {
                int color = GetColorByte(ResultMat, x, y);
                if (points.TryGetValue(color, out FoundPoint p))
                {
                    p.Add(x, y);
                }
                else
                {
                    points.Add(color, new FoundPoint(color, x, y));
                }
            }
        }

        FoundPoint point = points.Values.MaxBy(p => p.Count);
        if (point != null)
        {
            Point = new Point(point.X, point.Y);
            FilterValues.Add("Point", $"{point.X} {point.Y} {point.Count}");
        }

        Draw(dc =>
        {
            System.Windows.Media.Brush brush = System.Windows.Media.Brushes.GreenYellow;
            foreach (FoundPoint pair in points.Values.OrderByDescending(p => p.Count))
            {
                foreach (Point p in pair.Points)
                {
                    dc.DrawEllipse(brush, null, p, 3, 3);
                }
                brush = System.Windows.Media.Brushes.DarkRed;
            }
        });
    }

    private int GetColorByte(Mat image, int x, int y)
    {
        if (x < 0
            || y < 0
            || y > image.Rows
            || x > image.Cols)
        {
            return -1;
        }
        var rawData = image.GetRawData(y, x);
        return rawData[0];
    }
}