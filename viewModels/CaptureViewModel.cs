using Billiard.UI;
using Emgu.CV;
using System.Windows.Input;
using System.Windows.Media;
using Billiard.Threading;

namespace Billiard.viewModels
{
    public class CaptureViewModel : ViewModel
    {
        public VideoDeviceViewModel VideoDevice { get; }

        private ImageSource output;
        public ImageSource Output
        {
            get { return output; }
            set { SetProperty(ref output, value); }
        }

        public CaptureViewModel(VideoDeviceViewModel videoDevice)
        {
            VideoDevice = videoDevice;

/*            //the fps of the webcam
            int cameraFps = 30;
            timer = new Timer()
            {
                Interval = 1000 / cameraFps,
                Enabled = true
            };
            timer.Elapsed += new ElapsedEventHandler(timer_Tick);
*/        }

        private void Capturer_ImageGrabbed(object sender, System.EventArgs e)
        {
            var frame = VideoDevice.Capturer.QueryFrame();

            /*            //flip the image horizontally
                        CvInvoke.Flip(frame, frame, FlipType.Horizontal);
            */
            if (frame != null)
            {
                ThreadDispatcher.Invoke(() => Output = frame.ToBitmapSource());
            }

        }

        public ICommand StopCommand
        {
            get { return new TargetCommand(Stop); }
        }

        private void Stop()
        {
            if (VideoDevice.Capturer != null)
            {
                VideoDevice.Capturer.Stop();

                VideoDevice.Capturer.ImageGrabbed -= Capturer_ImageGrabbed;
            }
        }

        public ICommand StartCommand
        {
            get { return new TargetCommand(Start); }
        }
            
        private void Start()
        {
            if (VideoDevice.Capturer != null)
            {
                VideoDevice.Capturer.ImageGrabbed -= Capturer_ImageGrabbed;
                VideoDevice.Capturer.ImageGrabbed += Capturer_ImageGrabbed;

                VideoDevice.Capturer.Start();
            }
        }
        
    }
}
