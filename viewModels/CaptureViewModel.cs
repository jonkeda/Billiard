using System.Windows.Input;
using System.Windows.Media;
using Billiard.Camera.Devices;
using Billiard.UI;
using Emgu.CV;
using System.Collections.Generic;
using System.Linq;
using Billiard.Camera.vision;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;

namespace Billiard.viewModels
{
    internal class CaptureViewModel : ViewModel
    {
        private ImageSource grayTableImage;
        private ImageSource tableImage;
        private ImageSource floodFillImage;
        private ImageSource originalImage;
        private ImageSource cannyTableImage;
        private ImageSource hsvTableImage;
        private ImageSource hTableImage;
        private ImageSource sTableImage;
        private ImageSource vTableImage;
        private VideoDevice selectedVideoDevice;
        private VideoCapture capturer;

        public BilliardVisionEngine visionEngine = new();

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
            //Image<Bgr, Byte> img1 = new Image<Bgr, Byte>("C:\\Temp\\billiard.jpg");

            /*            Video.Source = img1.ToBitmapSource();
                        Video.Source = Camera.Camera.capture();
            */
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

            OriginalImage = image.ToBitmapSource();
            
            visionEngine.processFrame(image);

            FloodFillImage = visionEngine.engineState.floodfillMat?.ToBitmapSource();

            TableImage = visionEngine.engineState.tableMat?.ToBitmapSource();

            GrayTableImage = visionEngine.engineState.grayTableMat?.ToBitmapSource();
            CannyTableImage = visionEngine.engineState.binaryTableMat?.ToBitmapSource();

            HsvTableImage = visionEngine.engineState.hsvTableMat?.ToBitmapSource();
            HTableImage = visionEngine.engineState.hTableMat?.ToBitmapSource();
            STableImage = visionEngine.engineState.sTableMat?.ToBitmapSource();
            VTableImage = visionEngine.engineState.vTableMat?.ToBitmapSource();

/*                 Mat _gray = new Mat();
                 Mat _cannyEdges = new Mat();
 
                 //Convert the image to grayscale and filter out the noise
                 CvInvoke.CvtColor(image, _gray, ColorConversion.Bgr2Gray);
 
                 //Remove noise
                 CvInvoke.GaussianBlur(_gray, _gray, new System.Drawing.Size(3, 3), 1);
                 float cannyThreshold = 180.0f;
                 float cannyThresholdLinking = 120.0f;
                 CvInvoke.Canny(_gray, _cannyEdges, cannyThreshold, cannyThresholdLinking);
 
 
                 BinaryTableImage = _cannyEdges.ToBitmapSource();*/
/*            Video.Source = _cannyEdges.ToBitmapSource();
 
                 float circleAccumulatorThreshold = 120;
                 CircleF[] circles = CvInvoke.HoughCircles(_gray, HoughModes.Gradient, 2.0, 20.0, cannyThreshold,
                     circleAccumulatorThreshold, 5);
*/        }

    }
}
