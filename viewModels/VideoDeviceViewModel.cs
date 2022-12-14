using Billiard.Camera.Devices;
using Billiard.UI;
using Emgu.CV;
using System.Collections.Generic;
using System.Linq;

namespace Billiard.viewModels
{
    public class VideoDeviceViewModel : ViewModel
    {
        private VideoDevice selectedVideoDevice;
        private VideoCapture capturer;


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
                    capturer?.Dispose();
                    capturer = null;
                }
            }
        }

        public VideoCapture Capturer
        {
            get
            {
                if (capturer == null && SelectedVideoDevice != null)
                {
                    capturer = new VideoCapture(SelectedVideoDevice.Index);
                }
                return capturer;
            }
        }
    }
}
