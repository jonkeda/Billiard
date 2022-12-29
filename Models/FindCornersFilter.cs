using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Windows.Media;
using Billiard.Camera.vision.Geometries;
using Emgu.CV;
using Brushes = System.Windows.Media.Brushes;
using Pen = System.Windows.Media.Pen;
using Point = System.Windows.Point;

namespace Billiard.Models;

public class FindCornersFilter : AbstractFilter, IPointsFilter
{
    private IBoundingRectFilter boundingRect;
    private List<Point> points;
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
            #region Calculate Points

            Point? topLeft = FindFirstX(r.BoundingRect.Y + 1, r.BoundingRect, ResultMat);
            Point? topLeft2 = FindFirstXFromTop(topLeft, r.BoundingRect, ResultMat);

            //Point? topRight = FindLastX(r.BoundingRect.Y + 1, r.BoundingRect, ResultMat);
            //Point? topRight2 = FindFirstXFromTop(topRight, r.BoundingRect, ResultMat);

            Point? bottomLeft = FindFirstX(r.BoundingRect.Y + r.BoundingRect.Height - 1, r.BoundingRect, ResultMat);
            Point? bottomLeft2 = FindFirstXFromBottom(bottomLeft, r.BoundingRect, ResultMat);

            //Point? bottomRight = FindLastX(r.BoundingRect.Y + r.BoundingRect.Height, r.BoundingRect, ResultMat);
            //Point? bottomRight2 = FindFirstXFromBottom(bottomRight, r.BoundingRect, ResultMat);

            Point? leftTop = FindFirstY(r.BoundingRect.X + 1, r.BoundingRect, ResultMat);
            Point? leftTop2 = FindFirstYFromLeft(leftTop, r.BoundingRect, ResultMat);

            //Point? leftBottom = FindLastY(r.BoundingRect.X + 1, r.BoundingRect, ResultMat);
            //Point? leftBottom2 = FindFirstYFromLeft(leftBottom, r.BoundingRect, ResultMat);

            Point? rightTop = FindFirstY(r.BoundingRect.X + r.BoundingRect.Width - 1, r.BoundingRect, ResultMat);
            Point? rightTop2 = FindFirstYFromRight(rightTop, r.BoundingRect, ResultMat);

            //Point? rightBottom = FindLastY(r.BoundingRect.X + r.BoundingRect.Width, r.BoundingRect, ResultMat);
            //Point? rightBottom2 = FindFirstYFromRight(rightBottom, r.BoundingRect, ResultMat);

            #endregion

            List<Point> points = FindCorners(leftTop, leftTop2, topLeft, topLeft2, bottomLeft, bottomLeft2, rightTop,
                rightTop2);

            Points = points;

            #region draw

            int radius = Math.max(ResultMat.Cols / 100, ResultMat.Rows / 100);
/*            Pen blueColor = new Pen(Brushes.Blue, Math.max(ResultMat.Cols / 200, ResultMat.Rows / 200))
            {
                DashStyle = DashStyles.Solid
            };
            Pen redColor = new Pen(Brushes.Red, Math.max(ResultMat.Cols / 200, ResultMat.Rows / 200))
            {
                DashStyle = DashStyles.Solid
            };
*/
            Draw(dc =>
            {
                Pen examplePen = new Pen(Brushes.GreenYellow, 5)
                {
                    DashStyle = DashStyles.Solid
                };
                PathFigure figure = new PathFigure
                {
                    IsClosed = true,
                    StartPoint = points[0]
                };
                foreach (Point point in points.Skip(1))
                {
                    figure.Segments.Add(new LineSegment(point, true));
                }

                Geometry geometry = new PathGeometry(new List<PathFigure> { figure });
                dc.DrawGeometry(null, examplePen, geometry);

                DrawEllipse(dc, Brushes.Blue, null, topLeft, topLeft2, radius);
                // DrawEllipse(dc, Brushes.Blue, null, topRight, topRight2, radius);

                DrawEllipse(dc, Brushes.LightBlue, null, bottomLeft, bottomLeft2, radius);
                //DrawEllipse(dc, Brushes.Blue, null, bottomRight, bottomRight2, radius);

                DrawEllipse(dc, Brushes.Red, null, leftTop, leftTop2, radius);
                //DrawEllipse(dc, Brushes.Red, null, leftBottom, leftBottom2, radius);

                DrawEllipse(dc, Brushes.DeepPink, null, rightTop, rightTop2, radius);
                //DrawEllipse(dc, Brushes.Red, null, rightBottom, rightBottom2, radius);


            });
            #endregion

        }
    }

    private List<Point> FindCorners(Point? left1, Point? left2, Point? top1, Point? top2, Point? bottom1, Point? bottom2, Point? right1, Point? right2)
    {
        //RotatedRect rect = CvInvoke.MinAreaRect(pointsf.Select(i => new PointF(i.X, i.Y)).ToArray());

        Vector2? eLeft1 = PointFExtensions.Extend(left1, left2);
        Vector2? eLeft2 = PointFExtensions.Extend(left2, left1);

        Vector2? eTop1 = PointFExtensions.Extend(top1, top2);
        Vector2? eTop2 = PointFExtensions.Extend(top2, top1);

        Vector2? eRight1 = PointFExtensions.Extend(right1, right2);
        Vector2? eRight2 = PointFExtensions.Extend(right2, right1);

        Vector2? eBottom1 = PointFExtensions.Extend(bottom1, bottom2);
        Vector2? eBottom2 = PointFExtensions.Extend(bottom2, bottom1);

        Point? topLeft = null;
        Point? topRight = null;
        Point? bottomLeft = null;
        Point? bottomRight = null;
        List<Point> rectPoints = new List<Point>();
        Vector2 intersection;
        if (LineSegmentsIntersect(eTop1, eTop2, eLeft1, eLeft2, out intersection))
        {
            topLeft = intersection.AsPoint();
            rectPoints.Add(topLeft.Value);
        }
        if (LineSegmentsIntersect(eTop1, eTop2, eRight1, eRight2, out intersection))
        {
            topRight = intersection.AsPoint();
            rectPoints.Add(topRight.Value);
        }
        if (LineSegmentsIntersect(eBottom1, eBottom2, eRight1, eRight2, out intersection))
        {
            bottomRight = intersection.AsPoint();
            rectPoints.Add(bottomRight.Value);
        }
        if (LineSegmentsIntersect(eBottom1, eBottom2, eLeft1, eLeft2, out intersection))
        {
            bottomLeft = intersection.AsPoint();
            rectPoints.Add(bottomLeft.Value);
        }
        return rectPoints;
    }

    public static bool LineSegmentsIntersect(Vector2? pn, Vector2? p2n, Vector2? qn, Vector2? q2n,
    out Vector2 intersection, bool considerCollinearOverlapAsIntersect = false)
    {
        intersection = new Vector2();

        if (!pn.HasValue
            || !p2n.HasValue
            || !qn.HasValue
            || !q2n.HasValue)
        {
            return false;
        }
        Vector2 p = pn.Value;
        Vector2 p2 = p2n.Value;
        Vector2 q = qn.Value;
        Vector2 q2 = q2n.Value;

        Vector2 r = p2 - p;
        Vector2 s = q2 - q;
        float rxs = r.Cross(s);
        float qpxr = (q - p).Cross(r);

        // If r x s = 0 and (q - p) x r = 0, then the two lines are collinear.
        if (rxs.IsZero() && qpxr.IsZero())
        {
            // 1. If either  0 <= (q - p) * r <= r * r or 0 <= (p - q) * s <= * s
            // then the two lines are overlapping,
            /*                if (considerCollinearOverlapAsIntersect)
                                if ((0 <= (q - p) * r 
                                     && (q - p) * r <= r * r) 
                                    || (0 <= (p - q) * s && (p - q) * s <= s * s))
                                    return true;
            */
            // 2. If neither 0 <= (q - p) * r = r * r nor 0 <= (p - q) * s <= s * s
            // then the two lines are collinear but disjoint.
            // No need to implement this expression, as it follows from the expression above.
            return false;
        }

        // 3. If r x s = 0 and (q - p) x r != 0, then the two lines are parallel and non-intersecting.
        if (rxs.IsZero() && !qpxr.IsZero())
            return false;

        // t = (q - p) x s / (r x s)
        var t = (q - p).Cross(s) / rxs;

        // u = (q - p) x r / (r x s)

        var u = (q - p).Cross(r) / rxs;

        // 4. If r x s != 0 and 0 <= t <= 1 and 0 <= u <= 1
        // the two line segments meet at the point p + t r = q + u s.
        if (!rxs.IsZero() && (0 <= t && t <= 1) && (0 <= u && u <= 1))
        {
            // We can calculate the intersection point using either t or u.
            intersection = p + t * r;

            // An intersection was found.
            return true;
        }

        // 5. Otherwise, the two line segments are not parallel but do not intersect.
        return false;
    }

    private void DrawEllipse(DrawingContext dc, SolidColorBrush brush, Pen pen, Point? p1, Point? p2, int radius)
    {
        DrawEllipse(dc, brush, pen, p1, radius);
        DrawEllipse(dc, brush, pen, p2, radius);

        if (p1.HasValue && p2.HasValue)
        {
            dc.DrawLine(new Pen(brush, 2), p1.Value, p2.Value);
        }
    }

    private void DrawEllipse(DrawingContext dc, SolidColorBrush brush, Pen pen, Point? p, int radius)
    {
        if (p.HasValue)
        {
            dc.DrawEllipse(brush, pen, p.Value, radius, radius);
        }
    }

    private const int stepDivider = 5;

    private Point? FindFirstXFromTop(Point? p, Rectangle r, Mat mat)
    {
        if (!p.HasValue)
        {
            return null;
        }

        int step = r.Width / stepDivider;
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

    private Point? FindFirstXFromBottom(Point? p, Rectangle r, Mat mat)
    {
        if (!p.HasValue)
        {
            return null;
        }

        int step = r.Width / stepDivider;
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


    private Point? FindFirstYFromLeft(Point? p, Rectangle r, Mat mat)
    {
        if (!p.HasValue)
        {
            return null;
        }

        int step = r.Height / stepDivider;
        Point? p1 = null;
        if (p.Value.X > 1)
        {
            p1= FindFirstX((int)p.Value.Y + step, r, mat);
        }
        Point? p2 = FindFirstX((int)p.Value.Y - step, r, mat);
        if (!p1.HasValue || p1.Value.X <= 1)
            return p2;
        if (!p2.HasValue || p2.Value.X <= 1)
            return p1;
        if (p1.Value.X < p2.Value.X)
        {
            return p1;
        }

        return p2;
    }

    private Point? FindFirstYFromRight(Point? p, Rectangle r, Mat mat)
    {
        if (!p.HasValue)
        {
            return null;
        }

        int step = r.Height / stepDivider;
        Point? p1 = null;
        if (p.Value.X < mat.Cols - 1)
        {
            p1 = FindLastX((int)p.Value.Y + step, r, mat);
        }
        //Point? p1 = FindLastX((int)p.Value.Y + step, r, mat);
        Point? p2 = FindLastX((int)p.Value.Y - step, r, mat);
        if (!p1.HasValue || p1.Value.X >= mat.Cols - 1)
            return p2;
        if (!p2.HasValue || p1.Value.X >= mat.Cols - 1)
            return p1;
        if (p1.Value.X > p2.Value.X)
        {
            return p1;
        }

        return p2;
    }

    private Point? FindFirstX(int y, Rectangle r, Mat mat)
    {
        for (int x = r.Left; x < r.Right; x++)
        {
            int color = GetColorByte(mat, x, y);
            if (color == FindColor)
            {
                return new Point(x, y);
            }
        }
        return null;
    }

    private Point? FindLastX(int y, Rectangle r, Mat mat)
    {
        for (int x = r.Right; x > r.Left; x--)
        {
            int color = GetColorByte(mat, x, y);
            if (color == FindColor)
            {
                return new Point(x, y);
            }
        }
        return null;
    }

    private Point? FindFirstY(int x, Rectangle r, Mat mat)
    {
        for (int y = r.Top; y < r.Bottom; y++)
        {
            int color = GetColorByte(mat, x, y);
            if (color == FindColor)
            {
                return new Point(x, y);
            }
        }
        return null;
    }

    private Point? FindLastY(int x, Rectangle r, Mat mat)
    {
        for (int y = r.Bottom; y > r.Top; y--)
        {
            int color = GetColorByte(mat, x, y);
            if (color == FindColor)
            {
                return new Point(x, y);
            }
        }
        return null;
    }

    private int GetColorByte(Mat image, int x, int y)
    {
        if (x < 0
            || y < 0
            || y >= image.Rows
            || x >= image.Cols)
        {
            return -1;
        }
        var rawData = image.GetRawData(y, x);
        return rawData[0];
    }

    public List<Point> Points
    {
        get { return points; }
        set { SetProperty(ref points, value); }
    }
}