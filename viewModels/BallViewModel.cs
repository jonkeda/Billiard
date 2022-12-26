using System;
using System.Windows.Media;
using Billiard.UI;
using Emgu.CV;
using System.Windows;
using System.Windows.Media.Animation;
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

        private ImageSource contourImage;


        public TableDetector tableDetector = new();

        public BallDetector ballDetector
        {
            get { return ballDetector1; }
            set { SetProperty(ref ballDetector1, value); }
        }

        private ImageSource whiteBallImage;
        private ImageSource yellowBallImage;
        private ImageSource redBallImage;
        private ImageSource tablePointImage;
        private ImageSource hlsTableImage;
        private ImageSource h2TableImage;
        private ImageSource l2TableImage;
        private ImageSource hueImage;
        private ImageSource contourRectImage;
        private ImageSource histogramImage;
        private ImageSource redHistBallImage;
        private ImageSource whiteHistBallImage;
        private ImageSource yellowHistBallImage;
        private BallDetector ballDetector1 = new();

        public ImageSource OriginalImage
        {
            get { return originalImage; }
            private set { SetProperty(ref originalImage, value); }
        }

        public ImageSource ContourImage
        {
            get { return contourImage; }
            private set { SetProperty(ref contourImage, value); }
        }

        public ImageSource ContourRectImage
        {
            get { return contourRectImage; }
            private set { SetProperty(ref contourRectImage, value); }
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
        public ImageSource TablePointImage
        {
            get
            {
                return tablePointImage;
            }
            set { SetProperty(ref tablePointImage, value); }
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

        public ImageSource HistogramImage
        {
            get { return histogramImage; }
            set { SetProperty(ref histogramImage, value); }
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


        public ImageSource HueImage
        {
            get { return hueImage; }
            set { SetProperty(ref hueImage, value); }
        }


        public ImageSource HlsTableImage
        {
            get { return hlsTableImage; }
            set { SetProperty(ref hlsTableImage, value); }
        }

        public ImageSource H2TableImage
        {
            get { return h2TableImage; }
            set { SetProperty(ref h2TableImage, value); }
        }

        public ImageSource S2TableImage
        {
            get { return sTableImage; }
            set { SetProperty(ref sTableImage, value); }
        }

        public ImageSource L2TableImage
        {
            get { return l2TableImage; }
            set { SetProperty(ref l2TableImage, value); }
        }




        public ImageSource FoundBallsImage
        {
            get { return foundBallsImage; }
            set { SetProperty(ref foundBallsImage, value); }
        }

        public ImageSource FoundTablePointImage
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

        public ImageSource RedHistBallImage
        {
            get { return redHistBallImage; }
            set { SetProperty(ref redHistBallImage, value); }
        }

        public ImageSource WhiteHistBallImage
        {
            get { return whiteHistBallImage; }
            set { SetProperty(ref whiteHistBallImage, value); }
        }

        public ImageSource YellowHistBallImage
        {
            get { return yellowHistBallImage; }
            set { SetProperty(ref yellowHistBallImage, value); }
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

            if (tableDetector.tableMat?.GetData() != null)
            {
                ballDetector.Detect(tableDetector.tableMat);
                GrayTableImage = ballDetector.grayTableMat?.ToImageSource();
                CannyTableImage = ballDetector.cannyTableMat?.ToImageSource();

                HsvTableImage = ballDetector.hsvTableMat?.ToImageSource();
                HTableImage = ballDetector.hTableMat?.ToImageSource();
                STableImage = ballDetector.sTableMat?.ToImageSource();
                VTableImage = ballDetector.vTableMat?.ToImageSource();

                HlsTableImage = ballDetector.hlsTableMat?.ToImageSource();
                H2TableImage = ballDetector.h2TableMat?.ToImageSource();
                S2TableImage = ballDetector.s2TableMat?.ToImageSource();
                L2TableImage = ballDetector.l2TableMat?.ToImageSource();

                HueImage = ballDetector.hueMat?.ToImageSource();

                WhiteBallImage = ballDetector.whiteBallMat?.ToImageSource();
                YellowBallImage = ballDetector.yellowBallMat?.ToImageSource();
                RedBallImage = ballDetector.redBallMat?.ToImageSource();

                WhiteHistBallImage = DrawHist(ballDetector.whiteHistBallMat);
                YellowHistBallImage = DrawHist(ballDetector.yellowHistBallMat); 
                RedHistBallImage = DrawHist(ballDetector.redHistBallMat);

                FoundBallsImage = DrawBalls();
                FoundTablePointImage = TableViewModel.DrawFoundTable(FloodFillImage, tableDetector.floodFillPoints,
                    tableDetector.floodFillMPoints);
                TablePointImage = FoundTablePointImage;

                ContourImage = ballDetector.contourMat?.ToImageSource();
                ContourRectImage = ballDetector.contourRectMat?.ToImageSource();
                /*            physicsEngine.SetBalls(ballDetector.WhiteBallRelativePoint, ballDetector.YellowBallRelativePoint,
                                ballDetector.RedBallRelativePoint);
                */
                HistogramImage = DrawHist(ballDetector.histogramMat);

            }
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


        private DrawingImage DrawHist(Mat hist)
        {
            if (hist?.GetData() == null)
            {
                return null;
            }

            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext drawingContext = visual.RenderOpen())
            {
                drawingContext.PushClip(new RectangleGeometry(
                    new Rect(new Point(0, 0),
                        new Point(OriginalImage.Width, OriginalImage.Height))));
                drawingContext.DrawRectangle(new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 0, 0, 0)), null,
                    new Rect(0, 0, OriginalImage.Width, OriginalImage.Height));

                int width = 5;
                Pen color = new Pen(Brushes.Black, 5)
                {
                    DashStyle = DashStyles.Solid
                };

                float[,] data = (float[,])hist.GetData();
                for (int i = 0; i < data.Length; i++)
                {

                    drawingContext.DrawLine(color, new Point(i * width, 0),  new Point(i * width, (int)data[i, 0]));

                }

                drawingContext.Close();
            }

            return new DrawingImage(visual.Drawing);
        }
    }
}
