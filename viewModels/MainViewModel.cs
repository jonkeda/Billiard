using Billiard.Physics;
using Billiard.UI;

namespace Billiard.viewModels
{
    public class MainViewModel : ViewModel
    {
        private PhysicsEngine physicsEngine;
        private BallViewModel ballViewModel;
        private TableViewModel tableViewModel;
        private CaptureViewModel captureViewModel;

        public PhysicsEngine PhysicsEngine
        {
            get { return physicsEngine; }
            set { SetProperty(ref physicsEngine, value); }
        }

        public BallViewModel BallViewModel
        {
            get { return ballViewModel; }
            set { SetProperty(ref ballViewModel, value); }
        }

        public TableViewModel TableViewModel
        {
            get { return tableViewModel; }
            set { SetProperty(ref tableViewModel, value); }
        }

        public CaptureViewModel CaptureViewModel
        {
            get { return captureViewModel; }
            set { SetProperty(ref captureViewModel, value); }
        }

        public MainViewModel(TableViewModel tableViewModel, BallViewModel ballViewModel, CaptureViewModel captureViewModel)
        {
            TableViewModel = tableViewModel;
            BallViewModel = ballViewModel;
            CaptureViewModel = captureViewModel;
        }
        
    }
}
