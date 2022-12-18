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
using Point = System.Windows.Point;

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
        private ImageSource sameColorImage;

        public TableDetector tableDetector = new();
        private ImageSource sameColorPointImage;

        public ImageSource OriginalImage
        {
            get { return originalImage; }
            private set { SetProperty(ref originalImage, value); }
        }

        public ImageSource FloodFillImage
        {
            get { return floodFillImage; }
            set { SetProperty(ref floodFillImage, value); }
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
            get
            {
                return sameColorPointImage;
            }
            set { SetProperty(ref sameColorPointImage, value); }
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
    }
}
