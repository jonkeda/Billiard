using Billiard.Camera.Devices;
using Billiard.Physics;
using Billiard.UI;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Emgu.CV.CvEnum;
using Microsoft.Win32;
using Emgu.CV.Structure;

namespace Billiard.viewModels
{
    public class VideoDeviceViewModel : ViewModel
    {
        private VideoDevice selectedVideoDevice;
        private VideoCapture camera;
        
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
                    camera?.Dispose();
                    camera = null;
                }
            }
        }

        public VideoCapture Camera
        {
            get
            {
                if (camera == null && SelectedVideoDevice != null)
                {
                    camera = new VideoCapture(SelectedVideoDevice.Index);
                }
                return camera;
            }
        }

        public ICommand CaptureCommand
        {
            get { return new TargetCommand(Captures); }
        }

        private void Captures()
        {
            if (Camera == null
                || !Camera.Grab())
            {
                return;
            }

            Mat image = Camera.QueryFrame();
            if (image != null)
            {
                OnCaptureImage(image);
            }
        }

        public ICommand LoadCommand
        {
            get { return new TargetCommand(Load); }
        }

        private void Load()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                try
                {
                    Mat image = CvInvoke.Imread(ofd.FileName, ImreadModes.Color);
                    OnCaptureImage(image);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public event EventHandler<CaptureEvent> CaptureImage;

        protected void OnCaptureImage(Mat image)
        {
            if (image.Height > image.Width)
            {
                CvInvoke.ResizeForFrame(image, image, new System.Drawing.Size(500, 1000));
                //CvInvoke.Rotate(image, image, RotateFlags.Rotate90Clockwise);
            }
            else
            {
                CvInvoke.ResizeForFrame(image, image, new System.Drawing.Size(1000, 500));
            }
            CaptureImage?.Invoke(this, new CaptureEvent(image));
        }

        public void Stop()
        {
            if (Camera != null)
            {
                Camera.Stop();

                Camera.ImageGrabbed -= Capturer_ImageGrabbed;
            }

        }

        public void Start()
        {
            if (Camera != null)
            {
                Camera.ImageGrabbed -= Capturer_ImageGrabbed;
                Camera.ImageGrabbed += Capturer_ImageGrabbed;

                Camera.Start();
            }
        }

        private void Capturer_ImageGrabbed(object sender, EventArgs e)
        {
            OnCaptureImage(Camera.QueryFrame());
        }
    }

    public class CaptureEvent : EventArgs
    {
        public CaptureEvent(Mat image)
        {
            Image = image;
        }

        public Mat Image { get; }
    }

}
