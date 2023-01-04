using System.Collections.ObjectModel;

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
        public Table(Point? top, Point? bottom, Point? left, Point? right)
        {
            Top = top;
            Bottom = bottom;
            Left = left;
            Right = right;
        }

        public Point? Top { get; set; }
        public Point? Bottom { get; set; }
        public Point? Left { get; set; }
        public Point? Right { get; set; }
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
            Point? imagePoint, Point? actualPoint)
        {
            Color = color;
            ImagePoint = imagePoint;
            ActualPoint = actualPoint;
        }

        public BallColor Color { get; }
        public Point? ImagePoint { get; }
        public Point? ActualPoint { get; }
    }

    public class PointCollection : Collection<Point>
    {
    }


    public class Point
    {
        public Point()
        { }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }
    }

    public class TableDetectionResult
    {
        public TableDetectionResult(Table? table, BallCollection balls)
        {
            Table = table;
            Balls = balls;
        }

        public Table? Table { get; set; }

        public BallCollection Balls { get; set; }
    }

}