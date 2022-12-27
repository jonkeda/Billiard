using System.Drawing;
using System.Windows.Media;
using Billiard.Camera.vision.Geometries;
using Emgu.CV;
using Brushes = System.Windows.Media.Brushes;
using Pen = System.Windows.Media.Pen;
using Point = System.Windows.Point;

namespace Billiard.Models;

public class FindCornersFilter : AbstractFilter
{
    private IBoundingRectFilter boundingRect;
    public int FindColor { get; set; } = 255;

    public FindCornersFilter(AbstractFilter filter) : base(filter)
    {
        Name = "Find corners";
    }

    public IBoundingRectFilter BoundingRect
    {
        get
        {
            if (boundingRect == null)
            {
                return InputFilter as IBoundingRectFilter;
            }
            return boundingRect;
        }
        set { boundingRect = value; }
    }
    
    protected override void ApplyFilter(Mat originalImage)
    {
        ResultMat = GetInputMat();
        if (BoundingRect is IBoundingRectFilter r)
        {
            System.Windows.Point? topLeft = FindFirstX(r.BoundingRect.Y + 1, r.BoundingRect, ResultMat);
            System.Windows.Point? topLeft2 = FindFirstXFromTop(topLeft, r.BoundingRect, ResultMat);

            System.Windows.Point? topRight = FindLastX(r.BoundingRect.Y + 1, r.BoundingRect, ResultMat);
            System.Windows.Point? topRight2 = FindFirstXFromTop(topRight, r.BoundingRect, ResultMat);

            System.Windows.Point? bottomLeft = FindFirstX(r.BoundingRect.Y + r.BoundingRect.Height, r.BoundingRect, ResultMat);
            System.Windows.Point? bottomLeft2 = FindFirstXFromBottom(bottomLeft, r.BoundingRect, ResultMat);
            
            System.Windows.Point? bottomRight = FindLastX(r.BoundingRect.Y + r.BoundingRect.Height, r.BoundingRect, ResultMat);
            System.Windows.Point? bottomRight2 = FindFirstXFromBottom(bottomRight, r.BoundingRect, ResultMat);

            System.Windows.Point? leftTop = FindFirstY(r.BoundingRect.X + 1, r.BoundingRect, ResultMat);
            System.Windows.Point? leftTop2 = FindFirstYFromLeft(leftTop, r.BoundingRect, ResultMat);

            System.Windows.Point? leftBottom = FindLastY(r.BoundingRect.X + 1, r.BoundingRect, ResultMat);
            System.Windows.Point? leftBottom2 = FindFirstYFromLeft(leftBottom, r.BoundingRect, ResultMat);
            
            System.Windows.Point? rightTop = FindFirstY(r.BoundingRect.X + r.BoundingRect.Width, r.BoundingRect, ResultMat);
            System.Windows.Point? rightTop2 = FindFirstYFromRight(rightTop, r.BoundingRect, ResultMat);

            System.Windows.Point? rightBottom = FindLastY(r.BoundingRect.X + r.BoundingRect.Width, r.BoundingRect, ResultMat);
            System.Windows.Point? rightBottom2 = FindFirstYFromRight(rightBottom, r.BoundingRect, ResultMat);

            int radius = Math.max(ResultMat.Cols / 100, ResultMat.Rows / 100);
            Pen blueColor = new Pen(Brushes.Blue, Math.max(ResultMat.Cols / 200, ResultMat.Rows / 200))
            {
                DashStyle = DashStyles.Solid
            };
            Pen redColor = new Pen(Brushes.Red, Math.max(ResultMat.Cols / 200, ResultMat.Rows / 200))
            {
                DashStyle = DashStyles.Solid
            };
            Draw(dc =>
            {
                DrawEllipse(dc, Brushes.Blue, blueColor, topLeft, topLeft2, radius);
                // DrawEllipse(dc, Brushes.Blue, blueColor, topRight, topRight2, radius);

                DrawEllipse(dc, Brushes.Blue, redColor, bottomLeft, bottomLeft2, radius);
                //DrawEllipse(dc, Brushes.Blue, redColor, bottomRight, bottomRight2, radius);
                
                DrawEllipse(dc, Brushes.Red, blueColor, leftTop, leftTop2, radius);
                //DrawEllipse(dc, Brushes.Red, blueColor, leftBottom, leftBottom2, radius);
                
                DrawEllipse(dc, Brushes.Red, redColor, rightTop, rightTop2, radius);
                //DrawEllipse(dc, Brushes.Red, redColor, rightBottom, rightBottom2, radius);

            });
        }
    }

    private void DrawEllipse(DrawingContext dc, SolidColorBrush brush, Pen pen, Point? p1, Point? p2, int radius)
    {
        DrawEllipse(dc, brush, pen, p1, radius);
        DrawEllipse(dc, brush, pen, p2, radius);

        if (p1.HasValue && p2.HasValue)
        {
            dc.DrawLine(pen, p1.Value, p2.Value);
        }
    }

    private void DrawEllipse(DrawingContext dc, SolidColorBrush brush, Pen pen, Point? p, int radius)
    {
        if (p.HasValue)
        {
            dc.DrawEllipse(brush, pen, p.Value, radius, radius);
        }
    }
    private System.Windows.Point? FindFirstXFromTop(Point? p, Rectangle r, Mat mat)
    {
        if (!p.HasValue)
        {
            return null;
        }

        int step = r.Width / 10;
        Point? p1 = FindFirstY((int)p.Value.X + step, r, mat);
        Point? p2 = FindFirstY((int)p.Value.X - step, r, mat);
        if (!p1.HasValue)
            return p2;
        if (!p2.HasValue)
            return p1;
        if (p1.Value.Y < p2.Value.Y)
        {
            return p1;
        }

        return p2;
    }

    private System.Windows.Point? FindFirstXFromBottom(Point? p, Rectangle r, Mat mat)
    {
        if (!p.HasValue)
        {
            return null;
        }

        int step = r.Width / 10;
        Point? p1 = FindLastY((int)p.Value.X + step, r, mat);
        Point? p2 = FindLastY((int)p.Value.X - step, r, mat);
        if (!p1.HasValue)
            return p2;
        if (!p2.HasValue)
            return p1;
        if (p1.Value.Y > p2.Value.Y)
        {
            return p1;
        }

        return p2;
    }


    private System.Windows.Point? FindFirstYFromLeft(Point? p, Rectangle r, Mat mat)
    {
        if (!p.HasValue)
        {
            return null;
        }

        int step = r.Height / 10;
        Point? p1 = FindFirstX((int)p.Value.Y + step, r, mat);
        Point? p2 = FindFirstX((int)p.Value.Y - step, r, mat);
        if (!p1.HasValue || p1.Value.X == 1)
            return p2;
        if (!p2.HasValue || p2.Value.X == 1)
            return p1;
        if (p1.Value.X < p2.Value.X)
        {
            return p1;
        }

        return p2;
    }

    private System.Windows.Point? FindFirstYFromRight(Point? p, Rectangle r, Mat mat)
    {
        if (!p.HasValue)
        {
            return null;
        }

        int step = r.Height / 10;
        Point? p1 = FindLastX((int)p.Value.Y + step, r, mat);
        Point? p2 = FindLastX((int)p.Value.Y - step, r, mat);
        if (!p1.HasValue || p1.Value.X == mat.Cols - 2)
            return p2;
        if (!p2.HasValue || p1.Value.X == mat.Cols - 2)
            return p1;
        if (p1.Value.X > p2.Value.X)
        {
            return p1;
        }

        return p2;
    }

    private System.Windows.Point? FindFirstX(int y, Rectangle r, Mat mat)
    {
        for (int x = r.Left; x < r.Right; x++)
        {
            int color = GetColorByte(mat, x, y);
            if (color == FindColor)
            {
                return new System.Windows.Point(x, y);
            }
        }
        return null;
    }

    private System.Windows.Point? FindLastX(int y, Rectangle r, Mat mat)
    {
        for (int x = r.Right; x > r.Left; x--)
        {
            int color = GetColorByte(mat, x, y);
            if (color == FindColor)
            {
                return new System.Windows.Point(x, y);
            }
        }
        return null;
    }

    private System.Windows.Point? FindFirstY(int x, Rectangle r, Mat mat)
    {
        for (int y = r.Top; y < r.Bottom; y++)
        {
            int color = GetColorByte(mat, x, y);
            if (color == FindColor)
            {
                return new System.Windows.Point(x, y);
            }
        }
        return null;
    }

    private System.Windows.Point? FindLastY(int x, Rectangle r, Mat mat)
    {
        for (int y = r.Bottom; y > r.Top; y--)
        {
            int color = GetColorByte(mat, x, y);
            if (color == FindColor)
            {
                return new System.Windows.Point(x, y);
            }
        }
        return null;
    }

    private int GetColorByte(Mat image, int x, int y)
    {
        var rawData = image.GetRawData(y, x);
        return rawData[0];
    }

}