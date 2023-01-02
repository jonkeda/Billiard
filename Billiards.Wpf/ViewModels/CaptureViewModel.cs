using System.Collections.Generic;
using Billiard.UI;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using Pen = System.Windows.Media.Pen;
using Point = System.Windows.Point;
using System;
using System.Globalization;
using System.Linq;
using Billiards.Base.FilterSets;
using Billiards.Wpf.Extensions;
using OpenCvSharp;
using Rect = System.Windows.Rect;

namespace Billiard.viewModels
{
    public class CaptureViewModel : ViewModel
    {
        public VideoDeviceViewModel VideoDevice { get; }

        private Mat output;

        public Mat Output
        {
            get { return output; }
            set { SetProperty(ref output, value); }
        }

        private ImageSource overlay;

        public ImageSource Overlay
        {
            get { return overlay; }
            set { SetProperty(ref overlay, value); }
        }

        public CaptureViewModel(VideoDeviceViewModel videoDevice)
        {
            VideoDevice = videoDevice;
        }

/*        public System.Windows.Point ToRelativePoint(Rect frame, System.Windows.Point p)
        {
            if (p.X == 0 && p.Y == 0)
            {
                return p;
            }

            if (frame.Height > frame.Width)
            {
                return new System.Windows.Point(p.Y / frame.Height, 1 - (p.X / frame.Width));
            }
            return new System.Windows.Point(p.X / frame.Width, p.Y / frame.Height);
        }
*/
        public void CaptureImage(ResultModel result)
        {
            Output = result.Image;

            Mat frame = result.Image;
            List<Point2f> dest = result.Corners;
            List<Point2f> src = new List<Point2f>(new[]
                {
                    new Point2f(0, 0),
                    new Point2f(frame.Width, 0),
                    new Point2f(frame.Width, frame.Height),
                    new Point2f(0, frame.Height)
                }
            );
            Mat warpingMat = Cv2.GetPerspectiveTransform(src, dest);

            Overlay = DrawCaptureOverlay(frame, result.Corners.AsWindowsPoints(),
                WarpPerspective(warpingMat, result.WhiteBallPoint).AsWindowsPoint(),
                WarpPerspective(warpingMat, result.YellowBallPoint).AsWindowsPoint(),
                WarpPerspective(warpingMat, result.RedBallPoint).AsWindowsPoint(),
                result.Now);
        }

        public static Point2f? WarpPerspective(Mat warpingMat,
            Point2f? point)
        {
            if (!point.HasValue)
            {
                return null;
            }

            Point2f[] points = Cv2.PerspectiveTransform(new[] { point.Value }, warpingMat);
            return points[0];
        }

        private DrawingImage DrawCaptureOverlay(Mat frame, List<Point> tableCornerPoints,
            Point? whiteBallPoint, Point? yellowBallPoint, Point? redBallPoint, DateTime now)
        {
            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext drawingContext = visual.RenderOpen())
            {
                int width = frame.Width;
                int height = frame.Height;
                int widthSepBottom = frame.Width / 20;
                int widthSepTop = frame.Width / 5;
                int heightSep = frame.Width / 20;

                drawingContext.PushClip(new RectangleGeometry(
                    new Rect(new Point(0, 0),
                        new Point(width, height))));
                drawingContext.DrawRectangle(new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)), null,
                    new Rect(0, 0, width, height));

                DrawExample(widthSepTop, heightSep, width, widthSepBottom, height, drawingContext);

                DrawFoundTable(tableCornerPoints, drawingContext);
                if (tableCornerPoints.Count == 4)
                {
                    DrawBalls(whiteBallPoint, yellowBallPoint, redBallPoint, frame, drawingContext);
                }

                FormattedText formattedText = new(
                    (DateTime.Now - now).TotalMilliseconds.ToString("F0"),
                    CultureInfo.CurrentUICulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Verdana"),
                    32,
                    Brushes.AntiqueWhite, 1.25);

                drawingContext.DrawText(formattedText, new Point(0, 0));

                drawingContext.Close();
            }

            return new DrawingImage(visual.Drawing);
        }

        private void DrawBalls(Point? whiteBallPoint, Point? yellowBallPoint, Point? redBallPoint, Mat frame,
            DrawingContext drawingContext)
        {
            double radius = System.Math.Max(frame.Height / 50, frame.Width / 50);

            if (whiteBallPoint.HasValue)
            {
                Pen whiteColor = new Pen(Brushes.White, 5)
                {
                    DashStyle = DashStyles.Solid
                };
                drawingContext.DrawEllipse(null, whiteColor, whiteBallPoint.Value, radius, radius);
            }

            if (yellowBallPoint.HasValue)
            {
                Pen yellowColor = new Pen(Brushes.Yellow, 5)
                {
                    DashStyle = DashStyles.Solid
                };
                drawingContext.DrawEllipse(null, yellowColor, yellowBallPoint.Value, radius, radius);
            }

            if (redBallPoint.HasValue)
            {
                Pen redColor = new Pen(Brushes.DarkRed, 5)
                {
                    DashStyle = DashStyles.Solid
                };

                drawingContext.DrawEllipse(null, redColor, redBallPoint.Value, radius, radius);
            }
        }

        private static void DrawFoundTable(List<Point> tableCornerPoints,
                    DrawingContext drawingContext)
        {
            if (tableCornerPoints.Count == 0)
            {
                return;
            }

            Pen examplePen = new Pen(Brushes.GreenYellow, 5)
            {
                DashStyle = DashStyles.Solid
            };
            PathFigure figure = new PathFigure
            {
                IsClosed = true,
                StartPoint = tableCornerPoints[0]
            };
            foreach (Point point in tableCornerPoints.Skip(1))
            {
                figure.Segments.Add(new LineSegment(point, true));
            }

            Geometry geometry = new PathGeometry(new List<PathFigure> { figure });
            drawingContext.DrawGeometry(null, examplePen, geometry);

        }

        private static void DrawExample(int widthSepTop, int heightSep, int width, int widthSepBottom, int height,
            DrawingContext drawingContext)
        {
            Pen examplePen = new Pen(Brushes.Gray, 5)
            {
                DashStyle = DashStyles.Dash
            };
            Geometry geometry = new PathGeometry(new List<PathFigure>()
            {
                new PathFigure(new Point(widthSepTop, heightSep),
                    new List<PathSegment>
                    {
                        new LineSegment(new Point(width - widthSepTop, heightSep), true),
                        new LineSegment(new Point(width - widthSepBottom, height - heightSep), true),
                        new LineSegment(new Point(widthSepBottom, height - heightSep), true),
                    }, true)
            });
            drawingContext.DrawGeometry(null, examplePen, geometry);
        }

        public ICommand StopCommand
        {
            get { return new TargetCommand(Stop); }
        }

        private void Stop()
        {
            VideoDevice.Stop();
        }

        public ICommand StartCommand
        {
            get { return new TargetCommand(Start); }
        }

        private void Start()
        {
            VideoDevice.Start();
        }

    }
}
