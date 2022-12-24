using System.Windows.Media;
using Billiard.UI;
using Emgu.CV;
using Billiard.Camera.vision.Geometries;
using System.Windows;
using Billiard.Camera.vision.detectors;
using Brushes = System.Windows.Media.Brushes;
using Pen = System.Windows.Media.Pen;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Point = System.Windows.Point;
using Emgu.CV.Structure;
using System.Windows.Shapes;
using Brush = System.Windows.Media.Brush;

namespace Billiard.viewModels
{
    public class TableViewModel : ViewModel
    {
        public VideoDeviceViewModel VideoDevice { get; }

        public TableViewModel(VideoDeviceViewModel videoDevice)
        {
            VideoDevice = videoDevice;
            videoDevice.CaptureImage += VideoDevice_CaptureImage;
        }

        private ImageSource originalImage;
        private ImageSource grayTableImage;

        private ImageSource tableImage;
        private ImageSource floodFillImage;
        private ImageSource floodFillPointImage;
        private ImageSource cannyTableImage;
        private ImageSource hsvTableImage;
        private ImageSource hTableImage;
        private ImageSource sTableImage;
        private ImageSource vTableImage;
        private ImageSource inRangeImage;
        private ImageSource foundTableImage;
        private ImageSource foundLinesImage;
        private ImageSource sameColorImage;

        public TableDetector tableDetector = new();
        private ImageSource sameColorPointImage;
        private ImageSource tablePointImage;
        private ImageSource floodFillMaskImage;
        private ImageSource floodFillFoundLinesImage;
        private ImageSource hTableMidImage;
        private ImageSource inRangePointImage;
        private ImageSource foundOriginalImage;

        public ImageSource OriginalImage
        {
            get { return originalImage; }
            private set { SetProperty(ref originalImage, value); }
        }

        public ImageSource FoundOriginalImage
        {
            get { return foundOriginalImage; }
            private set { SetProperty(ref foundOriginalImage, value); }
        }

        public ImageSource TablePointImage
        {
            get { return tablePointImage; }
            set { SetProperty(ref tablePointImage, value); }
        }

        public ImageSource FloodFillImage
        {
            get { return floodFillImage; }
            set { SetProperty(ref floodFillImage, value); }
        }

        public ImageSource FloodFillMaskImage
        {
            get { return floodFillMaskImage; }
            set { SetProperty(ref floodFillMaskImage, value); }
        }

        public ImageSource FloodFillPointImage
        {
            get { return floodFillPointImage; }
            set { SetProperty(ref floodFillPointImage, value); }
        }

        public ImageSource InRangeImage
        {
            get { return inRangeImage; }
            set { SetProperty(ref inRangeImage, value); }
        }

        public ImageSource SameColorImage
        {
            get { return sameColorImage; }
            set { SetProperty(ref sameColorImage, value); }
        }

        public ImageSource SameColorPointImage
        {
            get { return sameColorPointImage; }
            set { SetProperty(ref sameColorPointImage, value); }
        }

        public ImageSource InRangePointImage
        {
            get { return inRangePointImage; }
            set { SetProperty(ref inRangePointImage, value); }
        }

        public ImageSource TableImage
        {
            get { return tableImage; }
            set { SetProperty(ref tableImage, value); }
        }

        public ImageSource GrayTableImage
        {
            get { return grayTableImage; }
            set { SetProperty(ref grayTableImage, value); }
        }

        public ImageSource CannyTableImage
        {
            get { return cannyTableImage; }
            set { SetProperty(ref cannyTableImage, value); }
        }

        public ImageSource HsvTableImage
        {
            get { return hsvTableImage; }
            set { SetProperty(ref hsvTableImage, value); }
        }

        public ImageSource HTableImage
        {
            get { return hTableImage; }
            set { SetProperty(ref hTableImage, value); }
        }

        public ImageSource HTableMidImage
        {
            get { return hTableMidImage; }
            set { SetProperty(ref hTableMidImage, value); }
        }

        public ImageSource STableImage
        {
            get { return sTableImage; }
            set { SetProperty(ref sTableImage, value); }
        }

        public ImageSource VTableImage
        {
            get { return vTableImage; }
            set { SetProperty(ref vTableImage, value); }
        }

        public ImageSource FoundTableImage
        {
            get { return foundTableImage; }
            set { SetProperty(ref foundTableImage, value); }
        }

        public ImageSource FoundLinesImage
        {
            get { return foundLinesImage; }
            set { SetProperty(ref foundLinesImage, value); }
        }

        public ImageSource FloodFillFoundLinesImage
        {
            get { return floodFillFoundLinesImage; }
            set { SetProperty(ref floodFillFoundLinesImage, value); }
        }

        private void VideoDevice_CaptureImage(object sender, CaptureEvent e)
        {
            Mat image = e.Image;
            if (image == null)
            {
                return;
            }

            OriginalImage = image.ToImageSource();

            tableDetector.Detect(image);

            FloodFillImage = tableDetector.floodFillMat?.ToImageSource();
            InRangeImage = tableDetector.inRangeMat?.ToImageSource();
            FloodFillMaskImage = tableDetector.floodFillMaskMat?.ToImageSource();

            SameColorImage = tableDetector.sameColorMat?.ToImageSource();

            TableImage = tableDetector.tableMat?.ToImageSource();

            GrayTableImage = tableDetector.grayTableMat?.ToImageSource();
            CannyTableImage = tableDetector.cannyTableMat?.ToImageSource();

            HsvTableImage = tableDetector.hsvTableMat?.ToImageSource();
            HTableImage = tableDetector.hTableMat?.ToImageSource();
            STableImage = tableDetector.sTableMat?.ToImageSource();
            VTableImage = tableDetector.vTableMat?.ToImageSource();


            FloodFillPointImage = DrawFoundTable(floodFillImage, tableDetector.floodFillPoints, tableDetector.floodFillMPoints);
            SameColorPointImage = DrawFoundTable(SameColorImage, tableDetector.sameColorPoints, tableDetector.sameColorMPoints);

            InRangePointImage = DrawFoundTable(floodFillImage, tableDetector.inRangePoints, tableDetector.inRangeMPoints);


            FoundLinesImage = DrawFoundLines(floodFillImage, tableDetector.HoughLines);

            FloodFillFoundLinesImage = DrawFoundLines(floodFillImage, tableDetector.FloodFillMatHoughLines);

            TablePointImage = sameColorPointImage;

            HTableMidImage = DrawMidpoint(HTableImage);

            FoundOriginalImage = DrawFoundOriginalTable(OriginalImage, tableDetector.inRangeMPoints);

        }


        public static DrawingImage DrawFoundOriginalTable(ImageSource image, List<PointF> tableCornerPoints)
        {
            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext drawingContext = visual.RenderOpen())
            {
                drawingContext.PushClip(new RectangleGeometry(
                    new Rect(new Point(0, 0), new Point(image.Width, image.Height))));
                drawingContext.DrawRectangle(new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 0, 0, 0)), null,
                    new Rect(0, 0, image.Width, image.Height));

                if (tableCornerPoints.Count == 0)
                {
                    return null;
                }

                Pen examplePen = new Pen(Brushes.GreenYellow, 5)
                {
                    DashStyle = DashStyles.Solid
                };
                PathFigure figure = new PathFigure
                {
                    IsClosed = true,
                    StartPoint = tableCornerPoints[0].AsPoint()
                };
                foreach (PointF pointF in tableCornerPoints.Skip(1))
                {
                    figure.Segments.Add(new LineSegment(pointF.AsPoint(), true));
                }

                Geometry geometry = new PathGeometry(new List<PathFigure> { figure });
                drawingContext.DrawGeometry(null, examplePen, geometry);


                drawingContext.Close();
            }

            return new DrawingImage(visual.Drawing);
        }

        public static DrawingImage DrawFoundTable(ImageSource image, List<PointF> points, List<PointF> pointsM)
        {
            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext drawingContext = visual.RenderOpen())
            {
                drawingContext.PushClip(new RectangleGeometry(
                    new Rect(new Point(0, 0), new Point(image.Width, image.Height))));
                drawingContext.DrawRectangle(new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 0, 0, 0)), null,
                    new Rect(0, 0, image.Width, image.Height));


                /*                drawingContext.DrawRectangle(new SolidColorBrush(System.Windows.Media.Colors.Aqua), null,
                                    new Rect(0, 0, image.Width/2, image.Height/2));
                */
                double radius = System.Math.Max(image.Height / 100, image.Width / 100);

                // drawingContext.DrawRectangle(new SolidColorBrush(System.Windows.Media.Colors.Aqua), null,
                //    new Rect(0, 0, image.Width, image.Height));

                /*                Pen forceColor = new Pen(Brushes.GreenYellow, 5)
                                {
                                    DashStyle = DashStyles.Solid
                                };
                */
                foreach (System.Drawing.PointF point in points)
                {
                    drawingContext.DrawEllipse(Brushes.GreenYellow, null, point.AsPoint(), radius, radius);
                }

                foreach (System.Drawing.PointF point in pointsM)
                {
                    drawingContext.DrawEllipse(Brushes.Red, null, point.AsPoint(), radius, radius);
                }

                drawingContext.Close();
            }

            return new DrawingImage(visual.Drawing);
        }

        public static DrawingImage DrawFoundLines(ImageSource image, LineSegment2D[] houghLines)
        {
            Pen pen = new Pen(Brushes.Aqua, 5)
            {
                DashStyle = DashStyles.Dot
            };

            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext drawingContext = visual.RenderOpen())
            {
                drawingContext.PushClip(new RectangleGeometry(
                    new Rect(new Point(0, 0), new Point(image.Width, image.Height))));
                drawingContext.DrawRectangle(new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 0, 0, 0)), null,
                    new Rect(0, 0, image.Width, image.Height));

                foreach (LineSegment2D line in houghLines)
                {
                    drawingContext.DrawLine(pen, line.P1.AsPoint(), line.P2.AsPoint());
                }
                DrawLine(drawingContext, Brushes.Purple, MostTop(houghLines));
                DrawLine(drawingContext, Brushes.Violet, MostBottom(houghLines));
                DrawLine(drawingContext, Brushes.DeepPink, MostLeft(houghLines));
                DrawLine(drawingContext, Brushes.Pink, MostRight(houghLines));

                drawingContext.Close();
            }

            return new DrawingImage(visual.Drawing);
        }

        public static void DrawLine(DrawingContext drawingContext, Brush brush, LineSegment2D? line)
        {
            Pen pen2 = new Pen(brush, 10)
            {
                DashStyle = DashStyles.Solid
            };
            if (line.HasValue)
            {
                drawingContext.DrawLine(pen2, line.Value.P1.AsPoint(), line.Value.P2.AsPoint());
            }
        }

        public static LineSegment2D? MostTop(LineSegment2D[] lines)
        {
            float top = float.MaxValue;
            LineSegment2D? found = null;
            foreach (LineSegment2D line in lines)
            {
                if (line.P1.Y < top)
                {
                    top = line.P1.Y;
                    found = line;
                }
                if (line.P2.Y < top)
                {
                    top = line.P2.Y;
                    found = line;
                }
            }

            return found;
        }

        public static LineSegment2D? MostBottom(LineSegment2D[] lines)
        {
            float bottom = float.MinValue;
            LineSegment2D? found = null;
            foreach (LineSegment2D line in lines)
            {
                if (line.P1.Y > bottom)
                {
                    bottom = line.P1.Y;
                    found = line;
                }
                if (line.P2.Y > bottom)
                {
                    bottom = line.P2.Y;
                    found = line;
                }
            }

            return found;
        }

        public static LineSegment2D? MostLeft(LineSegment2D[] lines)
        {
            float left = float.MaxValue;
            LineSegment2D? found = null;
            foreach (LineSegment2D line in lines)
            {
                if (line.P1.X < left)
                {
                    left = line.P1.X;
                    found = line;
                }
                if (line.P2.X < left)
                {
                    left = line.P2.X;
                    found = line;
                }
            }

            return found;
        }

        public static LineSegment2D? MostRight(LineSegment2D[] lines)
        {
            float right = float.MinValue;
            LineSegment2D? found = null;
            foreach (LineSegment2D line in lines)
            {
                if (line.P1.X > right)
                {
                    right = line.P1.X;
                    found = line;
                }
                if (line.P2.X > right)
                {
                    right = line.P2.X;
                    found = line;
                }
            }

            return found;
        }

        public static DrawingImage DrawMidpoint(ImageSource image)
        {
            Pen pen = new Pen(Brushes.Aqua, 5)
            {
                DashStyle = DashStyles.Dot
            };

            double radius = System. Math.Max(image.Height / 100, image.Width / 100);

            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext drawingContext = visual.RenderOpen())
            {
                drawingContext.PushClip(new RectangleGeometry(
                    new Rect(new Point(0, 0), new Point(image.Width, image.Height))));
                drawingContext.DrawRectangle(new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 0, 0, 0)), null,
                    new Rect(0, 0, image.Width, image.Height));

                drawingContext.DrawEllipse(Brushes.OrangeRed, null, new Point(image.Width / 2, image.Height / 2), radius, radius);

                drawingContext.Close();
            }

            return new DrawingImage(visual.Drawing);
        }



    }
}
