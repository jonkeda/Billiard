using Billiard.UI;
using Billiards.Base.Physics;

namespace Billiard.viewModels
{
    public class MainViewModel : ViewModel
    {
        public VideoDeviceViewModel VideoDevice { get; }
        public PhysicsEngine PhysicsEngine { get; }
        public CaptureViewModel CaptureViewModel { get; }
        public FilterViewModel FilterViewModel { get; }

        public MainViewModel(CaptureViewModel captureViewModel, FilterViewModel filterViewModel, 
            PhysicsEngine physicsEngine, VideoDeviceViewModel videoDevice)
        {
            CaptureViewModel = captureViewModel;
            PhysicsEngine = physicsEngine;
            FilterViewModel = filterViewModel;
            VideoDevice = videoDevice;
        }
    }
}
