using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Emgu.CV;
using Point = System.Windows.Point;

namespace Billiard.Models;

public class FindCornerHarrisFilter : AbstractFilter, IPointsFilter
{
    private List<Point> points;

    public List<Point> Points
    {
        get { return points; }
        set { SetProperty(ref points, value); }
    }

    public FindCornerHarrisFilter(AbstractFilter filter) : base(filter)
    {
        Name = "Find corner harris";
    }


    protected override void ApplyFilter(Mat originalImage)
    {
        // create corner strength image and do Harris
        var corners = new Mat();
        ResultMat = GetInputMat();
        CvInvoke.CornerHarris(ResultMat, corners, 2);
        CvInvoke.Normalize(corners, corners, 255, 0, Emgu.CV.CvEnum.NormType.MinMax);

        Matrix<float> matrix = new Matrix<float>(corners.Rows, corners.Cols);
        corners.CopyTo(matrix);

        List<Point> points = new List<Point>();
        float threshold = 200;
        for (int i = 0; i < matrix.Rows; i++)
        {
            for (int j = 0; j < matrix.Cols; j++)
            {
                if (matrix[i, j] > threshold)
                {
                    points.Add(new Point(j, i));
                    //CvInvoke.Circle(img, new Point(j, i), 5, new MCvScalar(0, 0, 255), 3);
                }
            }
        }

        #region draw

        
                    int radius = Math.Max(ResultMat.Cols / 100, ResultMat.Rows / 100);
                    Pen blueColor = new Pen(Brushes.Blue, Math.Max(ResultMat.Cols / 200, ResultMat.Rows / 200))
                    {
                        DashStyle = DashStyles.Solid
                    };
                    Pen redColor = new Pen(Brushes.Red, Math.Max(ResultMat.Cols / 200, ResultMat.Rows / 200))
                    {
                        DashStyle = DashStyles.Solid
                    };
                    Draw(dc =>
                    {
/*                        DrawEllipse(dc, Brushes.Blue, blueColor, topLeft, topLeft2, radius);
                        // DrawEllipse(dc, Brushes.Blue, blueColor, topRight, topRight2, radius);

                        DrawEllipse(dc, Brushes.Blue, redColor, bottomLeft, bottomLeft2, radius);
                        //DrawEllipse(dc, Brushes.Blue, redColor, bottomRight, bottomRight2, radius);

                        DrawEllipse(dc, Brushes.Red, blueColor, leftTop, leftTop2, radius);
                        //DrawEllipse(dc, Brushes.Red, blueColor, leftBottom, leftBottom2, radius);

                        DrawEllipse(dc, Brushes.Red, redColor, rightTop, rightTop2, radius);
                        //DrawEllipse(dc, Brushes.Red, redColor, rightBottom, rightBottom2, radius);
*/
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

                    });

        #endregion

    }

}

