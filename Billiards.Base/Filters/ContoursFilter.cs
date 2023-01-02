using OpenCvSharp;

namespace Billiards.Base.Filters;

public class ContoursFilter : AbstractFilter, IContourFilter
{
    private ContourType contourType = ContourType.Ellipse;

    public ContoursFilter(AbstractFilter filter) : base(filter)
    {
        Name = "Contours";
    }

    public ContourType ContourType
    {
        get { return contourType; }
        set { SetProperty(ref contourType, value); }
    }

    public double MinimumArea { get; set; } = -1;

    public double MaximumArea { get; set; } = -1;

    public double Resize { get; set; } = 1;

    public ContourCollection Contours { get; set; }

    public RetrievalModes RetrType { get; set; } = RetrievalModes.External;
    public ContourApproximationModes ChainApproxMethod { get; set; } = ContourApproximationModes.ApproxNone;

    protected override void ApplyFilter(Mat originalImage)
    {
        ResultMat = GetInputMat();

        // VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
        Point[][] contours;
        Cv2.FindContours(GetInputMat(), out contours, out _, RetrType,  ChainApproxMethod);

        FilterValues.Add("Contours", contours.Length);

        ContourCollection contourList = new ContourCollection();
        for (int i = 0; i < contours.Length; i++)
        {
            Point[] contour = contours[i];
            double area = Cv2.ContourArea(contour);
            FilterValues.Add($"Area {i}", area);
            if ((MinimumArea <= 0 || area > MinimumArea)
                && (MaximumArea <= 0 || area < MaximumArea))
            {
                contourList.Add(new Contour(contour));
            }
        }
        FilterValues.Add("Contours found", contourList.Count);
        if (ContourType == ContourType.ConvexHull)
        {
            List<Point> allPoints = new List<Point>();
            foreach (var contour in contourList)
            {
                allPoints.AddRange(contour.Points);
            }
            var points = Cv2.ConvexHull(allPoints);

            contourList.Clear();
            contourList.Add(new Contour(points));
        }
        else if (ContourType == ContourType.Ellipse)
        {
            foreach (Contour contour in contourList)
            {
                var rectangle = Cv2.FitEllipse(contour.Points);
                rectangle.Size = new Size2f((float)(rectangle.Size.Width * Resize), (float)(rectangle.Size.Height * Resize));
                contour.RotatedRectangle = rectangle;
            }
        }
        else if (ContourType == ContourType.Rectangle)
        {
            foreach (Contour contour in contourList)
            {
                var rectangle = Cv2.MinAreaRect(contour.Points);
                rectangle.Size = new Size2f((float)(rectangle.Size.Width * Resize), (float)(rectangle.Size.Height * Resize));
                contour.RotatedRectangle = rectangle;
            }
        }

        /*
                Draw(dc =>
                {
                    if (ContourType == ContourType.Points)
                    {
                        DrawAsPoints(contourList, dc);
                    }
                    else if (ContourType == ContourType.Circle)
                    {
                        DrawAsCircles(contourList, dc);
                    }
                    else if (ContourType == ContourType.Rectangle)
                    {
                        DrawAsRectangles(contourList, dc);
                    }
                    else if (ContourType == ContourType.Ellipse)
                    {
                        DrawAsEllipse(contourList, dc);
                    }
                    else if (ContourType == ContourType.ConvexHull)
                    {
                        DrawAsConvexHull(contourList, dc);
                    }
                    else if (ContourType == ContourType.ConvexHulls)
                    {
                        DrawAsConvexHulls(contourList, dc);
                    }

                });*/


        Contours = contourList;
    }
    /*
    private static void DrawAsConvexHull(ContourCollection contours, DrawingContext dc)
    {
        Pen examplePen = new Pen(Brushes.GreenYellow, 2)
        {
            DashStyle = DashStyles.Solid
        };
        List<Point2f> allPoints = new List<Point2f>();
        foreach (var contour in contours)
        {
            allPoints.AddRange(contour.Points);
        }

        var points = Cv2.ConvexHull(allPoints.AsPoint2fArray());

        PathFigure figure = new PathFigure
        {
            IsClosed = true,
            StartPoint = points[0].AsPoint()
        };
        for (int j = 1; j < points.Length; j++)
        {
            figure.Segments.Add(new LineSegment(points[j].AsPoint(), true));
        }

        Geometry geometry = new PathGeometry(new List<PathFigure> { figure });
        dc.DrawGeometry(null, examplePen, geometry);
        for (int j = 0; j < points.Length; j++)
        {
            dc.DrawEllipse(Brushes.Red, null, points[j].AsPoint(), 5, 5);
        }

        // todo
        contours.Clear();
        contours.Add(new Contour(points.AsListOfDrawingPoint()));
    }


    private static void DrawAsConvexHulls(ContourCollection contours, DrawingContext dc)
    {
        Pen examplePen = new Pen(Brushes.GreenYellow, 2)
        {
            DashStyle = DashStyles.Solid
        };
        foreach (var contour in contours)
        {
            var points = Cv2.ConvexHull(contour.AsArray());

            PathFigure figure = new PathFigure
            {
                IsClosed = true,
                StartPoint = points[0].AsPoint()
            };
            for (int j = 1; j < points.Length; j++)
            {
                figure.Segments.Add(new LineSegment(points[j].AsPoint(), true));
            }

            Geometry geometry = new PathGeometry(new List<PathFigure> { figure });
            dc.DrawGeometry(null, examplePen, geometry);
            for (int j = 0; j < points.Length; j++)
            {
                dc.DrawEllipse(Brushes.Red, null, points[j].AsPoint(), 5, 5);
            }
        }
    }


    private static void DrawAsPoints(ContourCollection contours, DrawingContext dc)
    {
        Pen examplePen = new Pen(Brushes.GreenYellow, 2)
        {
            DashStyle = DashStyles.Solid
        };
        foreach (var t in contours)
        {
            VectorOfPoint contour = t.AsVectorOfPoint();
            PathFigure figure = new PathFigure
            {
                IsClosed = true,
                StartPoint = contour[0].AsPoint()
            };
            for (int j = 1; j < contour.Size; j++)
            {
                figure.Segments.Add(new LineSegment(contour[j].AsPoint(), true));
            }

            Geometry geometry = new PathGeometry(new List<PathFigure> { figure });
            dc.DrawGeometry(null, examplePen, geometry);
            for (int j = 0; j < contour.Size; j++)
            {
                dc.DrawEllipse(Brushes.Red, null, contour[j].AsPoint(), 5, 5);
            }
        }
    }

    private static void DrawAsCircles(ContourCollection contours, DrawingContext dc)
    {
        Pen pen = new Pen(Brushes.GreenYellow, 5)
        {
            DashStyle = DashStyles.Solid
        };
        foreach (var t in contours)
        {
            VectorOfPoint contour = t.AsVectorOfPoint();
            for (int j = 1; j < contour.Size; j++)
            {
                var circle = Cv2.MinEnclosingCircle(contour);
                dc.DrawEllipse(null, pen, circle.Center.AsPoint(), circle.Radius, circle.Radius);
            }
        }
    }

    private void DrawAsRectangles(ContourCollection contours, DrawingContext dc)
    {
        Pen pen = new Pen(Brushes.GreenYellow, 5)
        {
            DashStyle = DashStyles.Solid
        };
        for (int i = 0; i < contours.Count; i++)
        {
            VectorOfPoint contour = contours[i].AsVectorOfPoint();
            if (contour.Size > 5)
            {
                RotatedRect rectangle = Cv2.MinAreaRect(contour);
                Point2f[] vertices = Array.ConvertAll(rectangle.GetVertices(), Point2f.Round);

                PathFigure figure = new PathFigure
                {
                    IsClosed = true,
                    StartPoint = vertices[0].AsPoint()
                };
                for (int j = 1; j < vertices.Length; j++)
                {
                    figure.Segments.Add(new LineSegment(vertices[j].AsPoint(), true));
                }

                Geometry geometry = new PathGeometry(new List<PathFigure> { figure });
                dc.DrawGeometry(null, pen, geometry);

                contours[i] = new Contour(vertices);
                rectangle.Size = new SizeF((float)(rectangle.Size.Width * Resize), (float)(rectangle.Size.Height * Resize));
                contours[i].RotatedRectangle = rectangle;
            }
        }
    }


    private void DrawAsEllipse(ContourCollection contours, DrawingContext dc)
    {
        Pen pen = new Pen(Brushes.GreenYellow, 5)
        {
            DashStyle = DashStyles.Solid
        };
        for (int i = 0; i < contours.Count; i++)
        {
            VectorOfPoint contour = contours[i].AsVectorOfPoint();
            if (contour.Size > 5)
            {
                RotatedRect rectangle = Cv2.FitEllipse(contour);
                Point2f[] vertices = Array.ConvertAll(rectangle.GetVertices(), Point2f.Round);

                PathFigure figure = new PathFigure
                {
                    IsClosed = true,
                    StartPoint = vertices[0].AsPoint()
                };
                for (int j = 1; j < vertices.Length; j++)
                {
                    figure.Segments.Add(new LineSegment(vertices[j].AsPoint(), true));
                }

                Geometry geometry = new PathGeometry(new List<PathFigure> { figure });
                dc.DrawGeometry(null, pen, geometry);

                contours[i] = new Contour(vertices);
                rectangle.Size = new SizeF((float)(rectangle.Size.Width * Resize), (float)(rectangle.Size.Height * Resize));
                contours[i].RotatedRectangle = rectangle;

            }
        }
    }

    */

}