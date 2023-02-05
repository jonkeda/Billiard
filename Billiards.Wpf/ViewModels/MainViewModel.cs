using System.Runtime.InteropServices;
using Billiards.Base.Filters;
using Billiards.Base.FilterSets;
using Billiards.Base.Physics;
using Billiards.Base.Threading;
using Billiards.Wpf.UI;

namespace Billiards.Wpf.ViewModels
{
    public class MainViewModel : ViewModel
    {
        public VideoDeviceViewModel VideoDevice { get; }
        public TableViewModel TableViewModel { get; }
        public PhysicsEngine PhysicsEngine { get; }
        public CaptureViewModel CaptureViewModel { get; }
        public FilterViewModel FilterViewModel { get; }
        public CaramboleDetector Detector { get; }
        public ValidationViewModel ValidationViewModel { get; }

        public MainViewModel(TableViewModel tableViewModel, 
            CaptureViewModel captureViewModel, 
            FilterViewModel filterViewModel,
            PhysicsEngine physicsEngine, 
            VideoDeviceViewModel videoDevice,
            ValidationViewModel validationViewModel)
        {
            TableViewModel = tableViewModel;
            CaptureViewModel = captureViewModel;
            PhysicsEngine = physicsEngine;
            FilterViewModel = filterViewModel;
            VideoDevice = videoDevice;
            ValidationViewModel = validationViewModel;

            Detector = new(true);

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

            ResultModel result = Detector.ApplyFilters(e.Image, null);

            if (VideoDevice.Calculate)
            {
                result.CueBallColor = e.Cueball;
                PhysicsEngine.CalculateSolutions(result);
            }

            ThreadDispatcher.Invoke(
                () =>
                {
                    TableViewModel.CaptureImage(result);
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