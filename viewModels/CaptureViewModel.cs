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
            videoDevice.CaptureImage += VideoDevice_CaptureImage;
        }

        private void VideoDevice_CaptureImage(object sender, CaptureEvent e)
        {
            Mat frame = e.Image;
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
