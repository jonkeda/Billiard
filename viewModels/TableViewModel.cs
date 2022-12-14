using System.Windows.Input;
using System.Windows.Media;
using Billiard.UI;
using Emgu.CV;
using Billiard.Camera.vision;
using Billiard.Camera.vision.Geometries;
using System.Windows;
using Brushes = System.Windows.Media.Brushes;
using Pen = System.Windows.Media.Pen;

namespace Billiard.viewModels
{
    public class TableViewModel : ViewModel
    {
        public VideoDeviceViewModel VideoDevice { get; }

        public TableViewModel(VideoDeviceViewModel videoDevice)
        {
            VideoDevice = videoDevice;
        }

        private ImageSource originalImage;
        private ImageSource grayTableImage;

        private ImageSource tableImage;
        private ImageSource floodFillImage;
        private ImageSource cannyTableImage;
        private ImageSource hsvTableImage;
        private ImageSource hTableImage;
        private ImageSource sTableImage;
        private ImageSource vTableImage;

        public TableDetector tableDetector = new();

        public ICommand CaptureCommand
        {
            get { return new TargetCommand(Captures); }
        }

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

        private ImageSource inRangeImage;
        private ImageSource foundTableImage;

        public ImageSource InRangeImage
        {
            get { return inRangeImage; }
            set { SetProperty(ref inRangeImage, value); }
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

        private void Captures()
        {
            if (VideoDevice.Capturer == null
                || !VideoDevice.Capturer.Grab())
            {
                return;
            }

            Mat image = VideoDevice.Capturer.QueryFrame();
            if (image == null)
            {
                return;
            }

            OriginalImage = image.ToImageSource();

            tableDetector.Detect(image);

            FloodFillImage = tableDetector.floodFillMat?.ToImageSource();
            InRangeImage = tableDetector.inRangeMat?.ToImageSource();

            TableImage = tableDetector.tableMat?.ToImageSource();

            GrayTableImage = tableDetector.grayTableMat?.ToImageSource();
            CannyTableImage = tableDetector.cannyTableMat?.ToImageSource();

            HsvTableImage = tableDetector.hsvTableMat?.ToImageSource();
            HTableImage = tableDetector.hTableMat?.ToImageSource();
            STableImage = tableDetector.sTableMat?.ToImageSource();
            VTableImage = tableDetector.vTableMat?.ToImageSource();

            FoundTableImage = DrawFoundTable(InRangeImage, tableDetector);

        }

        public static DrawingImage DrawFoundTable(ImageSource InRangeImage, TableDetector tableDetector)
        {
            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext drawingContext = visual.RenderOpen())
            {
                drawingContext.PushClip(new RectangleGeometry(
                    new Rect(new Point(0, 0),
                        new Point(InRangeImage.Width, InRangeImage.Height))));
                drawingContext.DrawRectangle(new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 0, 0, 0)), null,
                    new Rect(0, 0,
                        InRangeImage.Width, InRangeImage.Height));


                Pen forceColor = new Pen(Brushes.GreenYellow, 5)
                {
                    DashStyle = DashStyles.Solid
                };
                foreach (System.Drawing.PointF point in tableDetector.points)
                {
                    drawingContext.DrawEllipse(null, forceColor, point.AsPoint(), 5, 5);
                }

                drawingContext.Close();
            }

            return new DrawingImage(visual.Drawing);
        }
    }
}
