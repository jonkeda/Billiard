using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Billiards.Base.Filters;
using Billiards.Base.FilterSets;
using Billiards.Wpf.Extensions;
using Billiards.Wpf.UI;
using OpenCvSharp;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using Pen = System.Windows.Media.Pen;
using Point = System.Windows.Point;

namespace Billiards.Wpf.ViewModels
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

        public void CaptureImage(ResultModel result)
        {
            if (result.Image == null)
            {
                return;
            }
            Output = result.Image;

            List<System.Windows.Point> cornerPoints = new();
            if (result.Corners != null)
            {
                foreach (var corner in result.Corners)
                {
                    var p2 = ToAbsolutePoint(result.Image, corner);
                    cornerPoints.Add(p2);
                }
            }

            foreach (ResultBall ball in result.Balls)
            {
                ball.ImageAbsolutePoint = ToAbsolutePoint2f(result.Image, ball.ImageRelativePosition);
            }

            Overlay = DrawCaptureOverlay(result.Image, cornerPoints,
                result.Balls,
                result.Now);
        }

        public Point2f ToAbsolutePoint2f(Mat image, Point2f? p)
        {
            if (!p.HasValue)
            {
                return new(0, 0);
            }
            return new(p.Value.X * image.Width, p.Value.Y * image.Height);
        }

        public System.Windows.Point ToAbsolutePoint(Mat image, Point2f? p)
        {
            if (!p.HasValue)
            {
                return new(0, 0);
            }
            return new(p.Value.X * image.Width, p.Value.Y * image.Height);
        }

        private DrawingImage DrawCaptureOverlay(Mat frame, List<Point> tableCornerPoints,
            ResultBallCollection balls, DateTime now)
        {
            DrawingVisual visual = new();
            using (DrawingContext drawingContext = visual.RenderOpen())
            {
                int width = frame.Width;
                int height = frame.Height;
                int widthSepBottom = frame.Width / 20;
                int widthSepTop = frame.Width / 5;
                int heightSep = frame.Width / 20;

                drawingContext.PushClip(new RectangleGeometry(
                    new(new(0, 0),
                        new Point(width, height))));
                drawingContext.DrawRectangle(new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)), null,
                    new(0, 0, width, height));

               // DrawExample(widthSepTop, heightSep, width, widthSepBottom, height, drawingContext);

                DrawFoundTable(tableCornerPoints, drawingContext);
                if (tableCornerPoints.Count == 4)
                {
                    DrawBalls(balls, frame, drawingContext);
                }

                FormattedText formattedText = new(
                    (DateTime.Now - now).TotalMilliseconds.ToString("F0"),
                    CultureInfo.CurrentUICulture,
                    FlowDirection.LeftToRight,
                    new("Verdana"),
                    32,
                    Brushes.AntiqueWhite, 1.25);

                drawingContext.DrawText(formattedText, new(0, 0));

                drawingContext.Close();
            }

            return new(visual.Drawing);
        }

        private void DrawBalls(ResultBallCollection balls, Mat frame,
            DrawingContext drawingContext)
        {
            double radius = System.Math.Max(frame.Height / 50, frame.Width / 50);

            foreach (ResultBall ball in balls)
            {
                if (ball.ImageAbsolutePoint != null)
                {
                    //GetColor()
                    Pen color = new(GetColor(ball.Color), 5)
                    {
                        DashStyle = DashStyles.Solid
                    };
                    drawingContext.DrawEllipse(null, color, 
                        ball.ImageAbsolutePoint.AsWindowsPoint().Value,
                        radius, radius);
                }
            }
        }

        private Brush GetColor(BallColor color)
        {
            if (color == BallColor.White)
            {
                return Brushes.White;
            }
            if (color == BallColor.Yellow)
            {
                return Brushes.Yellow;
            }
            return Brushes.DarkRed;
        }

        private static void DrawFoundTable(List<Point> tableCornerPoints,
                    DrawingContext drawingContext)
        {
            if (tableCornerPoints.Count == 0)
            {
                return;
            }

            Pen examplePen = new(Brushes.GreenYellow, 5)
            {
                DashStyle = DashStyles.Solid
            };
            PathFigure figure = new()
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
            Pen examplePen = new(Brushes.Gray, 5)
            {
                DashStyle = DashStyles.Dash
            };
            Geometry geometry = new PathGeometry(new List<PathFigure>()
            {
                new(new(widthSepTop, heightSep),
                    new List<PathSegment>
                    {
                        new LineSegment(new(width - widthSepTop, heightSep), true),
                        new LineSegment(new(width - widthSepBottom, height - heightSep), true),
                        new LineSegment(new(widthSepBottom, height - heightSep), true),
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
