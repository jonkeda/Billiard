using System.Windows.Media;
using Billiard.UI;
using Emgu.CV;
using System.Windows;
using Billiard.Camera.vision.detectors;
using Billiard.Camera.vision.Geometries;
using Billiard.Physics;

namespace Billiard.viewModels
{
    public class BallViewModel : ViewModel
    {
        public VideoDeviceViewModel VideoDevice { get; }
        private readonly PhysicsEngine physicsEngine;

        public BallViewModel(VideoDeviceViewModel videoDevice, PhysicsEngine physicsEngine)
        {
            VideoDevice = videoDevice;
            videoDevice.CaptureImage += VideoDevice_CaptureImage;
            this.physicsEngine = physicsEngine;
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
        private ImageSource inRangeImage;
        private ImageSource foundTableImage;
        private ImageSource foundBallsImage;

        public TableDetector tableDetector = new();
        public BallDetector ballDetector = new();

        private ImageSource whiteBallImage;
        private ImageSource yellowBallImage;
        private ImageSource redBallImage;

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

        private void VideoDevice_CaptureImage(object sender, CaptureEvent e)
        {
            Mat image = e.Image;
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

            FoundBallsImage = DrawBalls();
            FoundTableImage = TableViewModel.DrawFoundTable(FloodFillImage, tableDetector.floodFillPoints, tableDetector.floodFillMPoints);


            physicsEngine.SetBalls(ballDetector.WhiteBallRelativePoint, ballDetector.YellowBallRelativePoint,
                ballDetector.RedBallRelativePoint);
        }

        private DrawingImage DrawBalls()
        {
            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext drawingContext = visual.RenderOpen())
            {
                drawingContext.PushClip(new RectangleGeometry(
                    new Rect(new Point(0, 0),
                        new Point(OriginalImage.Width, OriginalImage.Height))));
                drawingContext.DrawRectangle(new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 0, 0, 0)), null,
                    new Rect(0, 0, OriginalImage.Width, OriginalImage.Height));

                double radius = System.Math.Max(OriginalImage.Height / 100, OriginalImage.Width / 100);

                Pen whiteColor = new Pen(Brushes.Black, 5)
                {
                    DashStyle = DashStyles.Solid
                };
                drawingContext.DrawEllipse(Brushes.Wheat, whiteColor, ballDetector.WhiteBallPoint, radius, radius);

                Pen yellowColor = new Pen(Brushes.Black, 5)
                {
                    DashStyle = DashStyles.Solid
                };
                drawingContext.DrawEllipse(Brushes.Yellow, yellowColor, ballDetector.YellowBallPoint, radius, radius);

                Pen redColor = new Pen(Brushes.Black, 5)
                {
                    DashStyle = DashStyles.Solid
                };
                drawingContext.DrawEllipse(Brushes.Red, redColor, ballDetector.RedBallPoint, radius, radius);


                drawingContext.Close();
            }

            return new DrawingImage(visual.Drawing);
        }
    }
}
