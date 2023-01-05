using System.Collections.ObjectModel;
using System.Globalization;

namespace Billiards.Web.Shared
{
    public class ImageData
    {
        public ImageData(string data)
        {
            Data = data;
        }

        public string Data { get; set; }
    }
    
    public class Table
    {
        public Table(PointCollection corners)
        {
            Corners = corners;
        }
        public PointCollection Corners { get; }
    }

    public enum BallColor
    {
        Red = 0,
        Yellow = 1,
        White = 2
    }

    public class BallCollection : Collection<Ball>
    {
    }

    public class Ball
    {
        public Ball(BallColor color,
            Point? imagePoint, Point? tablePoint)
        {
            Color = color;
            ImagePoint = imagePoint;
            TablePoint = tablePoint;
        }

        public BallColor Color { get; }
        public Point? ImagePoint { get; }
        public Point? ImageAbsolutePoint { get; set; }
        public Point? TablePoint { get; }
    }

    public class PointCollection : Collection<Point>
    {
    }


    public class Point
    {
        public Point()
        { }

        public Point(float x, float y)
        {
            X = x;
            Y = y;
        }

        public float X { get; set; }
        public float Y { get; set; }

        public override string ToString()
        {
            return X.ToString("F1", CultureInfo.InvariantCulture) + "," +
                   Y.ToString("F1", CultureInfo.InvariantCulture);
        }
    }

    public class TableDetectionResult
    {
        public TableDetectionResult(Table? table, BallCollection? balls)
        {
            Table = table;
            Balls = balls;
        }

        public Table? Table { get; set; }

        public BallCollection? Balls { get; set; }
    }

}