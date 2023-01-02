using System.Collections.Generic;
using System.Drawing;
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
using Billiards.Base.Physics;
using Billiards.Base.Threading;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
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
            videoDevice.CaptureImage += VideoDevice_CaptureImage;
            videoDevice.StreamImage += VideoDevice_CaptureImage;
        }

        private volatile bool calculating = false;
        private void VideoDevice_CaptureImage(object sender, CaptureEvent e)
        {
            if (calculating)
            {
                return;
            }
            try
            {
                calculating = true;
                Mat frame = e.Image;
                CaptureImage(frame);
                //OldCaptureImage(frame);
            }
            catch
            {
                //
            }
            finally
            {
                calculating = false;
            }
        }
      

        public System.Windows.Point ToRelativePoint(Rect frame, System.Windows.Point p)
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


        private void CaptureImage(Mat frame)
        {
            ThreadDispatcher.Invoke(
                () =>
                {
                    Output = frame;
                });
/*
            (List<Point> corners, Rect tableSize,
                    Point? whiteBallPoint, Point? yellowBallPoint, Point? redBallPoint) =
                FilterViewModel.ApplyFilters(frame);

            List<PointF> tableCornerPoints = corners.AsListOfPointF();

            VectorOfPointF dest = new VectorOfPointF(tableCornerPoints.ToArray());
            VectorOfPointF src = new VectorOfPointF(new[]
                {
                    new PointF(0, 0),
                    new PointF(frame.Width, 0),
                    new PointF(frame.Width, frame.Height),
                    new PointF(0, frame.Height)
                }
            );
            Mat warpingMat = CvInvoke.GetPerspectiveTransform(src, dest);

            ThreadDispatcher.Invoke(
                () =>
                {
                    Overlay = DrawCaptureOverlay(frame, corners,
                        WarpPerspective(warpingMat, whiteBallPoint),
                        WarpPerspective(warpingMat, yellowBallPoint),
                        WarpPerspective(warpingMat, redBallPoint));
                });
            ThreadDispatcher.Invoke(
                () =>
                {
                    if (VideoDevice.Calculate
                        && whiteBallPoint.HasValue
                        && yellowBallPoint.HasValue
                        && redBallPoint.HasValue)
                    {
                        PhysicsEngine.SetBalls(
                            ToRelativePoint(tableSize, whiteBallPoint.Value),
                            ToRelativePoint(tableSize, yellowBallPoint.Value),
                            ToRelativePoint(tableSize, redBallPoint.Value));
                    }
                });*/
        }

/*        public static Point? WarpPerspective(Mat warpingMat,
            Point? point)
        {
            if (!point.HasValue)
            {
                return null;
            }

            PointF[] points = Cv2.PerspectiveTransform(new[] { point.Value.AsPointF() }, warpingMat);
            return points[0].AsPoint();
        }*/


        private DrawingImage DrawCaptureOverlay(Mat frame, List<Point> tableCornerPoints,
            Point? whiteBallPoint, Point? yellowBallPoint, Point? redBallPoint)
        {
            DateTime now = DateTime.Now;

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
                    (DateTime.Now - now).TotalMilliseconds.ToString(),
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
            double radius = System.Math.Max(frame.Height / 100, frame.Width / 100);

            if (whiteBallPoint.HasValue)
            {
                Pen whiteColor = new Pen(Brushes.Black, 5)
                {
                    DashStyle = DashStyles.Solid
                };
                drawingContext.DrawEllipse(Brushes.Wheat, whiteColor, whiteBallPoint.Value, radius, radius);
            }

            if (yellowBallPoint.HasValue)
            {
                Pen yellowColor = new Pen(Brushes.Black, 5)
                {
                    DashStyle = DashStyles.Solid
                };
                drawingContext.DrawEllipse(Brushes.Yellow, yellowColor, yellowBallPoint.Value, radius, radius);
            }

            if (redBallPoint.HasValue)
            {
                Pen redColor = new Pen(Brushes.Black, 5)
                {
                    DashStyle = DashStyles.Solid
                };

                drawingContext.DrawEllipse(Brushes.Red, redColor, redBallPoint.Value, radius, radius);
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
