using System.Collections.Generic;
using System.Drawing;
using Billiard.UI;
using Emgu.CV;
using System.Windows.Input;
using System.Windows.Media;
using Billiard.Threading;
using System.Windows;
using Billiard.Camera.vision.detectors;
using Billiard.Camera.vision.Geometries;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using Pen = System.Windows.Media.Pen;
using Point = System.Windows.Point;

namespace Billiard.viewModels
{
    public class CaptureViewModel : ViewModel
    {
        public TableDetector TableDetector { get; }

        public VideoDeviceViewModel VideoDevice { get; }

        private ImageSource output;
        public ImageSource Output
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
            TableDetector = new TableDetector();
        }

        private void VideoDevice_CaptureImage(object sender, CaptureEvent e)
        {

            Mat frame = e.Image;
            var (tableCornerPoints, sameColorMPoints) = TableDetector.DetectFast(frame);
            if (frame != null)
            {
                ThreadDispatcher.Invoke(
                    () =>
                    {
                        Output = frame.ToBitmapSource();
                        Overlay = DrawCaptureOverlay(frame, tableCornerPoints);
                    });
            }
        }
            
        private DrawingImage DrawCaptureOverlay(Mat frame, List<PointF> tableCornerPoints)
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

                drawingContext.Close();
            }

            return new DrawingImage(visual.Drawing);
        }

        private static void DrawFoundTable(List<PointF> tableCornerPoints,
            DrawingContext drawingContext)
        {
            Pen examplePen = new Pen(Brushes.GreenYellow, 5)
            {
                DashStyle = DashStyles.Solid
            };
            Geometry geometry = new PathGeometry(new List<PathFigure>()
            {
                new PathFigure(tableCornerPoints[0].AsPoint(),
                    new List<PathSegment>
                    {
                        new LineSegment(tableCornerPoints[1].AsPoint(), true),
                        new LineSegment(tableCornerPoints[3].AsPoint(), true),
                        new LineSegment(tableCornerPoints[2].AsPoint(), true),
                    }, true)
            });
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
