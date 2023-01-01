using System.Drawing;
using System.Numerics;
using Billiards.Base.Extensions;
using OpenCvSharp;

namespace Billiards.Base.Filters;

public class FindCornersFilter : AbstractFilter, IPointsFilter
{
    private IBoundingRectFilter boundingRect;

    private List<Point2f> points;
    public List<Point2f> Points
    {
        get { return points; }
        set { SetProperty(ref points, value); }
    }

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

            Point2f? topLeft = FindFirstX(r.BoundingRect.Y + 1, r.BoundingRect, ResultMat);
            Point2f? topLeft2 = FindFirstXFromTop(topLeft, r.BoundingRect, ResultMat);

            //Point2f? topRight = FindLastX(r.BoundingRect.Y + 1, r.BoundingRect, ResultMat);
            //Point2f? topRight2 = FindFirstXFromTop(topRight, r.BoundingRect, ResultMat);

            Point2f? bottomLeft = FindFirstX(r.BoundingRect.Y + r.BoundingRect.Height - 1, r.BoundingRect, ResultMat);
            Point2f? bottomLeft2 = FindFirstXFromBottom(bottomLeft, r.BoundingRect, ResultMat);

            //Point2f? bottomRight = FindLastX(r.BoundingRect.Y + r.BoundingRect.Height, r.BoundingRect, ResultMat);
            //Point2f? bottomRight2 = FindFirstXFromBottom(bottomRight, r.BoundingRect, ResultMat);

            Point2f? leftTop = FindFirstY(r.BoundingRect.X + 1, r.BoundingRect, ResultMat);
            Point2f? leftTop2 = FindFirstYFromLeft(leftTop, r.BoundingRect, ResultMat);

            //Point2f? leftBottom = FindLastY(r.BoundingRect.X + 1, r.BoundingRect, ResultMat);
            //Point2f? leftBottom2 = FindFirstYFromLeft(leftBottom, r.BoundingRect, ResultMat);

            Point2f? rightTop = FindFirstY(r.BoundingRect.X + r.BoundingRect.Width - 1, r.BoundingRect, ResultMat);
            Point2f? rightTop2 = FindFirstYFromRight(rightTop, r.BoundingRect, ResultMat);

            //Point2f? rightBottom = FindLastY(r.BoundingRect.X + r.BoundingRect.Width, r.BoundingRect, ResultMat);
            //Point2f? rightBottom2 = FindFirstYFromRight(rightBottom, r.BoundingRect, ResultMat);

            #endregion

            List<Point2f> points = FindCorners(leftTop, leftTop2, topLeft, topLeft2, bottomLeft, bottomLeft2, rightTop,
                rightTop2);

            Points = points;

            #region draw

            int radius = Math.Max(ResultMat.Cols / 100, ResultMat.Rows / 100);
            /*            Pen blueColor = new Pen(Brushes.Blue, Math.max(ResultMat.Cols / 200, ResultMat.Rows / 200))
                        {
                            DashStyle = DashStyles.Solid
                        };
                        Pen redColor = new Pen(Brushes.Red, Math.max(ResultMat.Cols / 200, ResultMat.Rows / 200))
                        {
                            DashStyle = DashStyles.Solid
                        };
            */

            #endregion

            #region  Draw

            /*
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
                            foreach (Point2f point in points.Skip(1))
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
            */
#endregion

        }
    }

    private List<Point2f> FindCorners(Point2f? left1, Point2f? left2, Point2f? top1, Point2f? top2, Point2f? bottom1, Point2f? bottom2, Point2f? right1, Point2f? right2)
    {
        //RotatedRect rect = Cv2.MinAreaRect(pointsf.Select(i => new Point2f(i.X, i.Y)).ToArray());

        Vec2f? eLeft1 = Vec2FExtensions.Extend(left1, left2);
        Vec2f? eLeft2 = Vec2FExtensions.Extend(left2, left1);

        Vec2f? eTop1 = Vec2FExtensions.Extend(top1, top2);
        Vec2f? eTop2 = Vec2FExtensions.Extend(top2, top1);

        Vec2f? eRight1 = Vec2FExtensions.Extend(right1, right2);
        Vec2f? eRight2 = Vec2FExtensions.Extend(right2, right1);

        Vec2f? eBottom1 = Vec2FExtensions.Extend(bottom1, bottom2);
        Vec2f? eBottom2 = Vec2FExtensions.Extend(bottom2, bottom1);

        Point2f? topLeft = null;
        Point2f? topRight = null;
        Point2f? bottomLeft = null;
        Point2f? bottomRight = null;
        List<Point2f> rectPoints = new List<Point2f>();
        Vec2f intersection;
        if (Vec2FExtensions.LineSegmentsIntersect(eTop1, eTop2, eLeft1, eLeft2, out intersection))
        {
            topLeft = intersection.AsPoint2f();
            rectPoints.Add(topLeft.Value);
        }
        if (Vec2FExtensions.LineSegmentsIntersect(eTop1, eTop2, eRight1, eRight2, out intersection))
        {
            topRight = intersection.AsPoint2f();
            rectPoints.Add(topRight.Value);
        }
        if (Vec2FExtensions.LineSegmentsIntersect(eBottom1, eBottom2, eRight1, eRight2, out intersection))
        {
            bottomRight = intersection.AsPoint2f();
            rectPoints.Add(bottomRight.Value);
        }
        if (Vec2FExtensions.LineSegmentsIntersect(eBottom1, eBottom2, eLeft1, eLeft2, out intersection))
        {
            bottomLeft = intersection.AsPoint2f();
            rectPoints.Add(bottomLeft.Value);
        }
        return rectPoints;
    }

/*    private void DrawEllipse(DrawingContext dc, SolidColorBrush brush, Pen pen, Point2f? p1, Point2f? p2, int radius)
    {
        DrawEllipse(dc, brush, pen, p1, radius);
        DrawEllipse(dc, brush, pen, p2, radius);

        if (p1.HasValue && p2.HasValue)
        {
            dc.DrawLine(new Pen(brush, 2), p1.Value, p2.Value);
        }
    }

    private void DrawEllipse(DrawingContext dc, SolidColorBrush brush, Pen pen, Point2f? p, int radius)
    {
        if (p.HasValue)
        {
            dc.DrawEllipse(brush, pen, p.Value, radius, radius);
        }
    }
*/
    private const int stepDivider = 5;

    private Point2f? FindFirstXFromTop(Point2f? p, Rect r, Mat mat)
    {
        if (!p.HasValue)
        {
            return null;
        }

        int step = r.Width / stepDivider;
        Point2f? p1 = FindFirstY((int)p.Value.X + step, r, mat);
        Point2f? p2 = FindFirstY((int)p.Value.X - step, r, mat);
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

    private Point2f? FindFirstXFromBottom(Point2f? p, Rect r, Mat mat)
    {
        if (!p.HasValue)
        {
            return null;
        }

        int step = r.Width / stepDivider;
        Point2f? p1 = FindLastY((int)p.Value.X + step, r, mat);
        Point2f? p2 = FindLastY((int)p.Value.X - step, r, mat);
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


    private Point2f? FindFirstYFromLeft(Point2f? p, Rect r, Mat mat)
    {
        if (!p.HasValue)
        {
            return null;
        }

        int step = r.Height / stepDivider;
        Point2f? p1 = null;
        if (p.Value.X > 1)
        {
            p1= FindFirstX((int)p.Value.Y + step, r, mat);
        }
        Point2f? p2 = FindFirstX((int)p.Value.Y - step, r, mat);
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

    private Point2f? FindFirstYFromRight(Point2f? p, Rect r, Mat mat)
    {
        if (!p.HasValue)
        {
            return null;
        }

        int step = r.Height / stepDivider;
        Point2f? p1 = null;
        if (p.Value.X < mat.Cols - 1)
        {
            p1 = FindLastX((int)p.Value.Y + step, r, mat);
        }
        //Point2f? p1 = FindLastX((int)p.Value.Y + step, r, mat);
        Point2f? p2 = FindLastX((int)p.Value.Y - step, r, mat);
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

    private Point2f? FindFirstX(int y, Rect r, Mat mat)
    {
        for (int x = r.Left; x < r.Right; x++)
        {
            int color = GetColorByte(mat, x, y);
            if (color == FindColor)
            {
                return new Point2f(x, y);
            }
        }
        return null;
    }

    private Point2f? FindLastX(int y, Rect r, Mat mat)
    {
        for (int x = r.Right; x > r.Left; x--)
        {
            int color = GetColorByte(mat, x, y);
            if (color == FindColor)
            {
                return new Point2f(x, y);
            }
        }
        return null;
    }

    private Point2f? FindFirstY(int x, Rect r, Mat mat)
    {
        for (int y = r.Top; y < r.Bottom; y++)
        {
            int color = GetColorByte(mat, x, y);
            if (color == FindColor)
            {
                return new Point2f(x, y);
            }
        }
        return null;
    }

    private Point2f? FindLastY(int x, Rect r, Mat mat)
    {
        for (int y = r.Bottom; y > r.Top; y--)
        {
            int color = GetColorByte(mat, x, y);
            if (color == FindColor)
            {
                return new Point2f(x, y);
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

        return image.Get<int>(x, y);
    }
}