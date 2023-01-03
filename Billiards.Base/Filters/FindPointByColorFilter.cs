using Billiards.Base.Drawings;
using OpenCvSharp;
using Point2f = OpenCvSharp.Point2f;

namespace Billiards.Base.Filters;

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
        public List<Point2f> Points { get; } = new();

        public void Add(int x, int y)
        {
            Count++;
            Points.Add(new Point2f(x, y));
        }
    }

    public Point2f Point2f { get; set; }

    public int Size { get; set; } = 10;
    public int Step { get; set; } = 5;

    public FindPointByColorFilter(AbstractFilter filter) : base(filter)
    {
        Name = "Find Point2f By Color Filter";
    }

    protected override void ApplyFilter(Mat originalImage)
    {
        
        ResultMat = GetInputMat();

        if (ResultMat == null)
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
            Point2f = new Point2f(point.X, point.Y);
            FilterValues.Add("Point2f", $"{point.X} {point.Y} {point.Count}");
        }

        
        Draw(dc =>
        {
            Brush brush = Brushes.GreenYellow;
            foreach (FoundPoint pair in points.Values.OrderByDescending(p => p.Count))
            {
                foreach (Point2f p in pair.Points)
                {
                    dc.DrawEllipse(brush, null, p, 3, 3);
                }
                brush = Brushes.DarkRed;
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

        return image.Get<int>(x, y);
    }
}