using System.Windows.Input;
using System.Windows.Media;
using Billiard.Camera.Devices;
using Billiard.UI;
using Emgu.CV;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Billiard.Camera.vision;
using Billiard.Camera.vision.Geometries;

namespace Billiard.viewModels
{
    internal class BallViewModel : ViewModel
    {
        private ImageSource originalImage;
        private ImageSource grayTableImage;
        private ImageSource tableImage;
        private ImageSource floodFillImage;
        private ImageSource cannyTableImage;
        private ImageSource hsvTableImage;
        private ImageSource hTableImage;
        private ImageSource sTableImage;
        private ImageSource vTableImage;
        private ImageSource inRangeImage;
        private ImageSource foundTableImage;
        private ImageSource foundBallsImage;

        private VideoDevice selectedVideoDevice;
        private VideoCapture capturer;

        public TableDetector tableDetector = new();
        public BallDetector ballDetector = new();
        private ImageSource whiteBallImage;
        private ImageSource yellowBallImage;
        private ImageSource redBallImage;

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

        public ImageSource FoundBallsImage
        {
            get { return foundBallsImage; }
            set { SetProperty(ref foundBallsImage, value); }
        }

        public ImageSource FoundTableImage
        {
            get { return foundTableImage; }
            set { SetProperty(ref foundTableImage, value); }
        }

        public ImageSource WhiteBallImage
        {
            get { return whiteBallImage; }
            set { SetProperty(ref whiteBallImage, value); }
        }

        public ImageSource YellowBallImage
        {
            get { return yellowBallImage; }
            set { SetProperty(ref yellowBallImage, value); }
        }

        public ImageSource RedBallImage
        {
            get { return redBallImage; }
            set { SetProperty(ref redBallImage, value); }
        }

        public IReadOnlyList<VideoDevice> VideoDevices
        {
            get
            {
                using SystemDevices systemDevice = new SystemDevices();
                IReadOnlyList<VideoDevice> devices = systemDevice.VideoDevices();
                SelectedVideoDevice = devices.LastOrDefault();
                return devices;
            }
        }

        public VideoDevice SelectedVideoDevice
        {
            get { return selectedVideoDevice; }
            set
            {
                if (SetProperty(ref selectedVideoDevice, value))
                {
                    capturer = null;
                }
            }
        }

        private void Captures()
        {
            if (selectedVideoDevice == null)
            {
                return;
            }

            if (capturer == null)
            {
                capturer = new VideoCapture(SelectedVideoDevice.Index);
            }

            if (capturer == null
                || !capturer.Grab())
            {
                return;
            }

            Mat image = capturer.QueryFrame();
            if (image == null)
            {
                return;
            }

            tableDetector.Detect(image);
            TableImage = tableDetector.originMat?.ToImageSource();
            OriginalImage = tableDetector.tableMat.ToImageSource();
            FloodFillImage = tableDetector.floodFillMat?.ToImageSource();
            InRangeImage = tableDetector.inRangeMat?.ToImageSource();

            ballDetector.Detect(tableDetector.tableMat);
            GrayTableImage = ballDetector.grayTableMat?.ToImageSource();
            CannyTableImage = ballDetector.cannyTableMat?.ToImageSource();

            HsvTableImage = ballDetector.hsvTableMat?.ToImageSource();
            HTableImage = ballDetector.hTableMat?.ToImageSource();
            STableImage = ballDetector.sTableMat?.ToImageSource();
            VTableImage = ballDetector.vTableMat?.ToImageSource();

            WhiteBallImage = ballDetector.whiteBallMat?.ToImageSource();
            YellowBallImage = ballDetector.yellowBallMat?.ToImageSource();
            RedBallImage = ballDetector.redBallMat?.ToImageSource();

            foundBallsImage = DrawBalls();
            FoundTableImage = TableViewModel.DrawFoundTable(InRangeImage, tableDetector);
        }

        private DrawingImage DrawBalls()
        {
            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext drawingContext = visual.RenderOpen())
            {
                drawingContext.PushClip(new RectangleGeometry(
                    new Rect(new System.Windows.Point(0, 0),
                        new System.Windows.Point(OriginalImage.Width, OriginalImage.Height))));
                drawingContext.DrawRectangle(new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 0, 0, 0)), null,
                    new Rect(0, 0, OriginalImage.Width, OriginalImage.Height));

                Pen whiteColor = new Pen(Brushes.Black, 5)
                {
                    DashStyle = DashStyles.Solid
                };
                drawingContext.DrawEllipse(Brushes.Wheat, whiteColor, ballDetector.whiteBallPoint, 10, 10);

                Pen yellowColor = new Pen(Brushes.Black, 5)
                {
                    DashStyle = DashStyles.Solid
                };
                drawingContext.DrawEllipse(Brushes.Yellow, yellowColor, ballDetector.yellowBallPoint, 10, 10);

                Pen redColor = new Pen(Brushes.Black, 5)
                {
                    DashStyle = DashStyles.Solid
                };
                drawingContext.DrawEllipse(Brushes.Red, redColor, ballDetector.redBallPoint, 10, 10);


                drawingContext.Close();
            }

            return new DrawingImage(visual.Drawing);
        }
    }
}
