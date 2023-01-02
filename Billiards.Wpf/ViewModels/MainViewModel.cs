using Billiard.UI;
using Billiards.Base.FilterSets;
using Billiards.Base.Threading;
using System;
using Billiard.Physics;

namespace Billiard.viewModels
{
    public class MainViewModel : ViewModel
    {
        public VideoDeviceViewModel VideoDevice { get; }
        public PhysicsEngine PhysicsEngine { get; }
        public CaptureViewModel CaptureViewModel { get; }
        public FilterViewModel FilterViewModel { get; }
        public CaramboleDetector Detector { get; }

        public MainViewModel(CaptureViewModel captureViewModel, FilterViewModel filterViewModel,
            PhysicsEngine physicsEngine, VideoDeviceViewModel videoDevice)
        {
            CaptureViewModel = captureViewModel;
            PhysicsEngine = physicsEngine;
            FilterViewModel = filterViewModel;
            VideoDevice = videoDevice;

            Detector = new CaramboleDetector();

            videoDevice.CaptureImage += VideoDeviceCaptureImage;
            videoDevice.StreamImage += VideoDeviceStreamImage;
        }

        private volatile bool calculating = false;

        private void VideoDeviceCaptureImage(object? sender, CaptureEvent e)
        {
            if (calculating)
            {
                return;
            }

            ResultModel result = new ResultModel
            {
                Image = e.Image,
                Detector = Detector,
                Now = DateTime.Now
            };

            Detector.ApplyFilters(result);

            if (VideoDevice.Calculate)
            {
                PhysicsEngine.CalculateSolutions(result);
            }

            ThreadDispatcher.Invoke(
                () =>
                {
                    CaptureViewModel.CaptureImage(result);
                    FilterViewModel.CaptureImage(result);
                });
        }

        private void VideoDeviceStreamImage(object? sender, CaptureEvent e)
        {
            if (calculating)
            {
                return;
            }

        }

    }
}